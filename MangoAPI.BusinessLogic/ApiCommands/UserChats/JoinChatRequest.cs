﻿namespace MangoAPI.BusinessLogic.ApiCommands.UserChats
{
    using Newtonsoft.Json;

    public record JoinChatRequest
    {
        [JsonConstructor]
        public JoinChatRequest(string chatId)
        {
            ChatId = chatId;
        }

        public string ChatId { get; }
    }

    public static class JoinChatCommandMapper
    {
        public static JoinChatCommand ToCommand(this JoinChatRequest model, string userId)
        {
            return new ()
            {
                ChatId = model.ChatId,
                UserId = userId,
            };
        }
    }
}
