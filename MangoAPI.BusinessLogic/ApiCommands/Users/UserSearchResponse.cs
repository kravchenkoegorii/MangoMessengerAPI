﻿namespace MangoAPI.BusinessLogic.ApiCommands.Users
{
    using System.Collections.Generic;
    using System.Linq;
    using MangoAPI.BusinessLogic.Models;
    using MangoAPI.BusinessLogic.Responses;
    using MangoAPI.Domain.Constants;
    using MangoAPI.Domain.Entities;

    public record UserSearchResponse : ResponseBase<UserSearchResponse>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public List<UserSearchResult> Users { get; init; }

        public static UserSearchResponse FromSuccess(IEnumerable<UserEntity> users)
        {
            return new ()
            {
                Message = ResponseMessageCodes.Success,
                Success = true,
                Users = users.OrderBy(x => x.DisplayName)
                    .Select(x => new UserSearchResult
                    {
                        Username = x.UserName,
                        DisplayName = x.DisplayName,
                        Bio = x.Bio,
                        Image = x.Image,
                    }).ToList(),
            };
        }
    }
}
