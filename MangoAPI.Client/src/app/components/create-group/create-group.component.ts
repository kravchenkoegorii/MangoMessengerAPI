import {Component, OnInit} from '@angular/core';
import {CommunitiesService} from "../../services/communities.service";
import {CreateChannelCommand} from "../../types/requests/CreateChannelCommand";
import {Router} from "@angular/router";
import {ErrorNotificationService} from "../../services/error-notification.service";

@Component({
  selector: 'app-create-group',
  templateUrl: './create-group.component.html',
  styleUrls: ['./create-group.component.scss']
})
export class CreateGroupComponent{

  constructor(private _communitiesService: CommunitiesService,
              private _router: Router,
              private _errorNotificationService: ErrorNotificationService) { }

  public chatTitle: string = '';
  public chatDescription: string = '';

  onCreateChatClick(): void {
    let command = new CreateChannelCommand(this.chatTitle, this.chatDescription);
    this._communitiesService.createChannel(command).subscribe(_ => {
      this._router.navigateByUrl("app?methodName=chats").then(r => r);
    }, error => {
      this._errorNotificationService.notifyOnError(error);
    });
  }
}
