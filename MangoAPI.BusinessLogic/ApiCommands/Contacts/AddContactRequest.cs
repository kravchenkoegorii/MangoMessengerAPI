﻿namespace MangoAPI.BusinessLogic.ApiCommands.Contacts
{
    using Newtonsoft.Json;

    public record AddContactRequest
    {
        [JsonConstructor]
        public AddContactRequest(string contactId)
        {
            ContactId = contactId;
        }

        public string ContactId { get; }
    }

    public static class AddContactRequestMapper
    {
        public static AddContactCommand ToCommand(this AddContactRequest model, string userId)
        {
            return new ()
            {
                ContactId = model.ContactId,
                UserId = userId,
            };
        }
    }
}
