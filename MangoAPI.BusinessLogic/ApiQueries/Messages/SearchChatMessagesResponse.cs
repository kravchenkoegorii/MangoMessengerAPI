﻿using System.Collections.Generic;
using System.Linq;
using MangoAPI.BusinessLogic.Models;
using MangoAPI.BusinessLogic.Responses;
using MangoAPI.Domain.Constants;
using MangoAPI.Domain.Entities;

namespace MangoAPI.BusinessLogic.ApiQueries.Messages
{
    public record SearchChatMessagesResponse : ResponseBase<SearchChatMessagesResponse>
    {
        public List<Message> Messages { get; init; }

        public static SearchChatMessagesResponse FromSuccess(IEnumerable<MessageEntity> messages, UserEntity user) =>
            new()
            {
                Messages = messages.Select(message =>
                    new Message
                    {
                        MessageId = message.Id,
                        UserDisplayName = message.User.DisplayName,
                        MessageText = message.Content,
                        CreatedAt = message.CreatedAt.ToShortTimeString(),
                        UpdatedAt = message.UpdatedAt?.ToShortTimeString(),
                        Self = message.User.Id == user.Id,
                        IsEncrypted = message.IsEncrypted,
                        AuthorPublicKey = message.AuthorPublicKey,
                    }).ToList(),
                Message = ResponseMessageCodes.Success,
                Success = true
            };
    }
}