import {Component, OnDestroy, OnInit} from '@angular/core';
import {TokensService} from "../../services/messenger/tokens.service";
import {Chat} from "../../types/models/Chat";
import {CommunitiesService} from "../../services/api/communities.service";
import {ErrorNotificationService} from "../../services/messenger/error-notification.service";
import {Router} from "@angular/router";
import {formatDate} from "@angular/common";
import {MessagesService} from "../../services/api/messages.service";
import {Message} from "../../types/models/Message";
import {CommunityType} from "../../types/enums/CommunityType";
import {RoutingConstants} from "../../types/constants/RoutingConstants";
import {UserChatsService} from "../../services/api/user-chats.service";
import {ValidationService} from "../../services/messenger/validation.service";
import {SendMessageCommand} from "../../types/requests/SendMessageCommand";
import * as signalR from '@microsoft/signalr';
import {environment} from "../../../environments/environment";
import {EditMessageNotification} from "../../types/models/EditMessageNotification";
import {DeleteMessageNotification} from "../../types/models/DeleteMessageNotification";
import {Subject, takeUntil} from "rxjs";
import {DisplayNameColours} from 'src/app/types/enums/DisplayNameColours';
import {User} from 'src/app/types/models/User';
import {UsersService} from 'src/app/services/api/users.service';

@Component({
  selector: 'app-chats',
  templateUrl: './chats.component.html',
  styleUrls: ['./chats.component.scss']
})
export class ChatsComponent implements OnInit, OnDestroy {

  constructor(private _tokensService: TokensService,
              private _communitiesService: CommunitiesService,
              private _userChatsService: UserChatsService,
              private _messagesService: MessagesService,
              private _usersService: UsersService,
              private _errorNotificationService: ErrorNotificationService,
              private _router: Router,
              private _validationService: ValidationService) {
  }

  private connectionBuilder: signalR.HubConnectionBuilder = new signalR.HubConnectionBuilder();
  private connection: signalR.HubConnection = this.connectionBuilder
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(environment.baseUrl + 'notify')
    .build();
  private signalRConnected = false;
  public realTimeConnections: string[] = [];

  public userId: string | undefined = '';
  public chats: Chat[] = [];
  public userChats: Chat[] = [];

  public activeChat: Chat = {
    lastMessageId: "",
    lastMessageAuthor: "",
    lastMessageText: "",
    lastMessageTime: "",
    updatedAt: "",
    roleId: 1,
    communityType: CommunityType.PublicChannel,
    description: "",
    chatId: "",
    chatLogoImageUrl: "",
    isArchived: false,
    isMember: false,
    membersCount: 0,
    title: ""
  };

  public activeUser: User = {
    userId: '',
    displayName: '',
    displayNameColour: 0,
    birthdayDate: '',
    email: '',
    website: '',
    username: '',
    bio: '',
    address: '',
    facebook: '',
    twitter: '',
    instagram: '',
    linkedIn: '',
    publicKey: 0,
    pictureUrl: '',
  };

  public activeChatId: string = '';
  public messages: Message[] = [];

  public messageText: string = '';
  public searchChatQuery: string = '';
  public searchMessagesQuery: string = '';
  public chatFilter: string = 'All chats';

  componentDestroyed$: Subject<boolean> = new Subject()

  public get routingConstants(): typeof RoutingConstants {
    return RoutingConstants;
  }

  public get displayNameColours(): typeof DisplayNameColours {
    return DisplayNameColours;
  }

  ngOnInit(): void {
    this.initializeView();
  }

  initializeView(): void {
    let tokens = this._tokensService.getTokens();

    if (!tokens) {
      this._router.navigateByUrl(this.routingConstants.Login).then(r => r);
      return;
    }

    this.userId = tokens.userId;

    this._usersService.getUserById(this.userId).pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: response => {
        this.activeUser = response.user;
      },
      error: error => {
        this._errorNotificationService.notifyOnError(error);
      }
    });

    this._communitiesService.getUserChats().pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: response => {
        this.chats = response.chats.filter(x => !x.isArchived);
        this.userChats = this.chats;

        if (this.connection.state !== signalR.HubConnectionState.Connected) {
          this.connectChatsToHub();
        }

        if (!this.signalRConnected) {
          this.setSignalRMethods();
          this.signalRConnected = true;
        }
      },
      error: error => {
        this._errorNotificationService.notifyOnError(error);
      }
    });
  }

  connectChatsToHub(): void {
    this.connection.start().then(() => {
      this.chats.forEach(x => {
        if (this.realTimeConnections.includes(x.chatId)) {
          return;
        }

        this.connection.invoke("JoinGroup", x.chatId).then(() => this.realTimeConnections.push(x.chatId));
      });

      if (this.userId != null && this.realTimeConnections.includes(this.userId)) {
        return;
      }

      this.connection.invoke("JoinGroup", this.userId).then(r => r);

    }).catch(err => console.error(err.toString()));
  }

  setSignalRMethods(): void {
    this.connection.on("BroadcastMessageAsync", (message: Message) => this.onBroadcastMessage(message));

    this.connection.on("UpdateUserChatsAsync", (chat: Chat) => this.chats.push(chat));

    this.connection.on('NotifyOnMessageDeleteAsync', (notification: DeleteMessageNotification) => {
        let message = this.messages.filter(x => x.messageId === notification.messageId)[0];

        if (message.messageId === this.activeChat.lastMessageId) {
          this.activeChat.lastMessageAuthor = notification.newLastMessageAuthor;
          this.activeChat.lastMessageText = notification.newLastMessageText;
          this.activeChat.lastMessageTime = notification.newLastMessageTime;
          this.activeChat.lastMessageId = notification.newLastMessageId;
        }

        this.messages = this.messages.filter(x => x.messageId !== notification.messageId);
      }
    );

    this.connection.on('NotifyOnMessageEditAsync', (notification: EditMessageNotification) => {
      let message = this.messages.filter(x => x.messageId === notification.messageId)[0];

      if (message) {
        message.messageText = notification.modifiedText;
        message.updatedAt = notification.updatedAt;
      }

      if (notification.isLastMessage) {
        this.activeChat.lastMessageText = notification.modifiedText;
        this.activeChat.lastMessageTime = notification.updatedAt;
      }
    });
  }

  onJoinChatClick(): void {
    this._userChatsService.joinCommunity(this.activeChatId).pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: _ => {
        this.chats = this.userChats;
        this.chats.push(this.activeChat);
        this.chats.sort((chat1, chat2) => {
          let chat1LastMessageTime = new Date(chat1.lastMessageTime)
          let chat2LastMessageTime = new Date(chat2.lastMessageTime)
          if (chat1LastMessageTime > chat2LastMessageTime) {
            return 1;
          }

          if (chat1LastMessageTime < chat2LastMessageTime) {
            return -1;
          }

          return 0;
        });
        this.searchChatQuery = '';
        this.chatFilter = 'All chats';
        this.activeChat.isMember = true;
      }
    });
  }

  onBroadcastMessage(message: Message): void {
    message.self = message.userId == this.userId;
    let chat = this.chats.filter(x => x.chatId === message.chatId)[0];
    chat.lastMessageAuthor = message.userDisplayName;
    chat.lastMessageText = message.messageText;
    chat.lastMessageTime = message.createdAt;
    chat.lastMessageId = message.messageId;
    this.chats = this.chats.filter(x => x.chatId !== message.chatId);
    this.chats = [chat, ...this.chats];

    const includesMessage = this.messages.some(x => x.messageId === message.messageId);

    if (message.chatId === this.activeChatId && !includesMessage) {
      this.messages.push(message);
    }

    this.scrollToEnd();
  }

  onEmojiClick(event: Event): void {
    let button = event.currentTarget as HTMLButtonElement;
    let emoji = button.innerText;
    this.messageText += emoji;
  }

  toShortTimeString(date: string): string {
    return formatDate(date, "hh:mm a", "en-US");
  }

  getChatMessages(chatId: string | null): void {
    if (!chatId) {
      return;
    }

    this._messagesService.getChatMessages(chatId).pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: response => {
        this.messages = response.messages;
        this.scrollToEnd();
      },
      error: error => {
        this._errorNotificationService.notifyOnError(error);
      }
    });
  }

  loadChat(chatId: string): void {
    this.activeChatId = chatId;
    this.activeChat = this.chats.filter(x => x.chatId === this.activeChatId)[0];
    this.getChatMessages(this.activeChatId);
  }

  chatContainsMessages(chat: Chat): boolean {
    return chat.lastMessageAuthor != null && chat.lastMessageText != null;
  }

  onSearchChatQueryChange(): void {
    if (this.searchChatQuery) {
      this._communitiesService.searchChat(this.searchChatQuery).pipe(takeUntil(this.componentDestroyed$)).subscribe({
        next: response => {
          this.chatFilter = 'Search results';
          this.chats = response.chats;
        },
        error: error => {
          this._errorNotificationService.notifyOnError(error);
        }
      });
    } else {
      this.chatFilter = 'All chats';
      this.initializeView();
    }
  }

  onSearchMessageQueryChange(): void {
    if (this.searchMessagesQuery) {
      this._messagesService.searchMessages(this.activeChatId, this.searchMessagesQuery).pipe(takeUntil(this.componentDestroyed$))
        .subscribe({
          next: response => {
            this.messages = response.messages;
          },
          error: error => {
            this._errorNotificationService.notifyOnError(error);
          }
        });
    } else {
      this.getChatMessages(this.activeChatId);
    }
  }

  onChatFilterClick(event: Event): void {
    let div = event.currentTarget as HTMLDivElement;
    this.chatFilter = div.innerText;

    this._communitiesService.getUserChats().pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: response => {
        let chats = response.chats;
        switch (this.chatFilter) {
          case 'All chats':
            this.chats = chats;
            break;
          case 'Groups':
            this.chats = chats.filter(x => x.communityType === CommunityType.PublicChannel);
            break;
          case 'Direct chats':
            this.chats = chats.filter(x => x.communityType === CommunityType.DirectChat);
            break;
          case 'Archived':
            this.chats = chats.filter(x => x.isArchived);
            break;
        }

        this.searchChatQuery = '';
      },
      error: error => {
        this._errorNotificationService.notifyOnError(error);
      }
    });
  }

  onSearchMessageClick(): void {
    this._messagesService.searchMessages(this.activeChatId, this.searchMessagesQuery)
      .pipe(takeUntil(this.componentDestroyed$))
      .subscribe({
        next: response => {
          this.messages = response.messages;
          this.searchMessagesQuery = '';
        },
        error: error => {
          this._errorNotificationService.notifyOnError(error);
        }
      });
  }

  onLeaveChatClick(): void {
    this._userChatsService.leaveCommunity(this.activeChatId).pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: _ => {
        this.activeChatId = '';
        this.initializeView();
        this.userChats = this.userChats.filter(x => x !== this.activeChat);
      },
      error: error => {
        this._errorNotificationService.notifyOnError(error);
      }
    });
  }

  onArchiveChatClick(): void {
    this._userChatsService.archiveCommunity(this.activeChatId).pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: _ => {
        this.initializeView();
        this.activeChatId = '';
      },
      error: error => {
        this._errorNotificationService.notifyOnError(error);
      }
    });
  }

  onSendMessageClick(): void {
    this._validationService.validateField(this.messageText, 'Message Text Field');
    const tokens = this._tokensService.getTokens();

    if (!tokens) {
      alert('User tokens are empty.');
      return;
    }

    const isoString = new Date().toISOString();
    const messageId = crypto.randomUUID();

    const newMessage = new Message(
      messageId,
      tokens.userId,
      this.activeChatId,
      tokens.userDisplayName,
      this.activeUser.displayNameColour,
      this.messageText,
      isoString,
      true,
      tokens.userProfilePictureUrl);

    this.messages.push(newMessage);

    const sendMessageCommand = new SendMessageCommand(this.messageText, this.activeChatId);
    sendMessageCommand.setMessageId(messageId);
    sendMessageCommand.setCreatedAt(isoString);

    this._messagesService.sendMessage(sendMessageCommand).pipe(takeUntil(this.componentDestroyed$)).subscribe({
      next: data => {
        newMessage.messageId = data.messageId;
        this.clearMessageInput();
        this.scrollToEnd();
      },
      error: error => {
        this._errorNotificationService.notifyOnError(error);
      }
    });
  }

  onEnterClick(event: any): void {
    event.preventDefault();
    this.onSendMessageClick();
  }

  private clearMessageInput(): void {
    this.messageText = '';
  }

  scrollToEnd(): void {
    setTimeout(() => {
      let chatMessages = document.getElementById('chatMessages');
      if (!chatMessages) {
        return;
      }
      chatMessages.scrollTop = chatMessages.scrollHeight;
    })
  }

  getDisplayNameColour(colour: number): string {
    switch (colour) {
      case DisplayNameColours.White:
        return "color-white";
      case DisplayNameColours.Blue:
        return "color-blue";
      case DisplayNameColours.Red:
        return "color-red";
      case DisplayNameColours.Yellow:
        return "color-yellow";
      case DisplayNameColours.Green:
        return "color-green";
      case DisplayNameColours.BrightYellow:
        return "color-bright-yellow";
      case DisplayNameColours.Aqua:
        return "color-aqua";
      case DisplayNameColours.Violet:
        return "color-violet";
      case DisplayNameColours.Pink:
        return "color-pink";
      case DisplayNameColours.Orange:
        return "color-orange";
      default:
        return "color-pink";
    }
  }

  ngOnDestroy(): void {
    this.connection.stop().then(r => r);
    this.componentDestroyed$.next(true);
    this.componentDestroyed$.complete();
  }
}
