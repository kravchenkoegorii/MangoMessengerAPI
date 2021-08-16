﻿using Newtonsoft.Json;

namespace MangoAPI.BusinessLogic.ApiCommands.Chats
{
    public record SearchChatsRequest
    {
        [JsonConstructor]
        public SearchChatsRequest(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; }
    }

    public static class SearchChatsRequestMapper
    {
        public static SearchChatsCommand ToCommand(this SearchChatsRequest request, string userId)
        {
            return new()
            {
                DisplayName = request.DisplayName,
                UserId = userId
            };
        }
    }
}