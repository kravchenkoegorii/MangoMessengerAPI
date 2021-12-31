﻿using AutoMapper;
using MangoAPI.BusinessLogic.ApiCommands.Users;
using MangoAPI.BusinessLogic.ApiQueries.Users;
using MangoAPI.BusinessLogic.Responses;
using MangoAPI.Presentation.Extensions;
using MangoAPI.Presentation.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MangoAPI.Presentation.Controllers
{
    /// <summary>
    /// Controller responsible for User Entity.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ApiControllerBase, IUsersController
    {
        public UsersController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {
        }

        /// <summary>
        /// Registers user in the system.
        /// Does not require any authorization or users role.
        /// After registration user receives pair of access/refresh tokens.
        /// Access token claim role is Unverified.
        /// </summary>
        /// <param name="request">Request instance.</param>
        /// <param name="cancellationToken">Cancellation token instance.</param>
        /// <returns>Possible codes: 200, 400, 409.</returns>
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Description =
                "Registers user in the system. " +
                "Does not require any authorization or users role. " +
                "After registration user receives pair of access/refresh tokens. " +
                "Access token claim role is Unverified. ",
            Summary = "Registers user in the system.")]
        [ProducesResponseType(typeof(TokensResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var command = Mapper.Map<RegisterCommand>(request);
            return await RequestAsync(command, cancellationToken);
        }

        /// <summary>
        /// Confirms user's email address. Adds a User role to the current user.
        /// This endpoint may be accessed by both roles: Unverified, User.
        /// On refresh session user receives new access token with updated roles.
        /// </summary>
        /// <param name="request">VerifyEmailRequest instance.</param>
        /// <param name="cancellationToken">CancellationToken instance.</param>
        /// <returns>Possible codes: 200, 400, 409.</returns>
        [HttpPut("email-confirmation")]
        [AllowAnonymous]
        [SwaggerOperation(Description = "Confirms user's email address. Adds a User role to the current user. " +
                                        "This endpoint may be accessed by both roles: Unverified, User. " +
                                        "On refresh session user receives new access token with updated roles.",
            Summary = "Confirms user's email address.")]
        [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EmailConfirmationAsync([FromBody] VerifyEmailRequest request,
            CancellationToken cancellationToken)
        {
            var command = Mapper.Map<VerifyEmailCommand>(request);
            return await RequestAsync(command, cancellationToken);
        }

        /// <summary>
        /// Changes password by current password. Required role: User.
        /// </summary>
        /// <param name="request">Request instance.</param>
        /// <param name="cancellationToken">Cancellation Token Instance.</param>
        /// <returns></returns>
        [HttpPut("password")]
        [Authorize]
        [SwaggerOperation(Description = "Changes password by current password. Required role: User",
            Summary = "Changes password by current password.")]
        [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();

            var command = Mapper.Map<ChangePasswordCommand>(request);
            command.UserId = userId;

            return await RequestAsync(command, cancellationToken);
        }

        /// <summary>
        /// Gets user by ID. Requires role: User.
        /// </summary>
        /// <param name="userId">ID of the user to get, UUID.</param>
        /// <param name="cancellationToken">CancellationToken instance.</param>
        /// <returns>Possible codes: 200, 400, 409.</returns>
        [HttpGet("{userId:guid}")]
        [Authorize]
        [SwaggerOperation(Description = "Gets user by ID. Requires role: User.",
            Summary = "Gets user by ID.")]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetUserQuery
            {
                UserId = userId
            };

            return await RequestAsync(query, cancellationToken);
        }

        /// <summary>
        /// Updates user's social network user names. Requires role: User.
        /// </summary>
        /// <param name="request">UpdateUserSocialInformationRequest instance.</param>
        /// <param name="cancellationToken">CancellationToken instance.</param>
        /// <returns></returns>
        [HttpPut("socials")]
        [Authorize]
        [SwaggerOperation(Description = "Updates user's social network user names. Requires role: User.",
            Summary = "Updates user's social network user names.")]
        [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUserSocialInformationAsync(
            [FromBody] UpdateUserSocialInformationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();

            var command = Mapper.Map<UpdateUserSocialInformationCommand>(request);
            command.UserId = userId;

            return await RequestAsync(command, cancellationToken);
        }

        /// <summary>
        /// Updates user's personal account information. Requires role: User.
        /// </summary>
        /// <param name="request">UpdateUserInformationRequest instance.</param>
        /// <param name="cancellationToken">CancellationToken instance.</param>
        /// <returns>Possible codes: 200, 400, 409.</returns>
        [HttpPut("account")]
        [Authorize]
        [SwaggerOperation(Description = "Updates user's personal account information. Requires role: User.",
            Summary = "Updates user's personal account information.")]
        [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUserAccountInfoAsync([FromBody] UpdateUserAccountInfoRequest request,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();

            var command = Mapper.Map<UpdateUserAccountInfoCommand>(request);
            command.UserId = userId;

            return await RequestAsync(command, cancellationToken);
        }

        [HttpPost("picture")]
        [Authorize]
        [SwaggerOperation(Description = "Updates user's profile picture. Requires role: User.",
            Summary = "Updates user's profile picture.")]
        [ProducesResponseType(typeof(UpdateProfilePictureResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateProfilePictureAsync(IFormFile pictureFile,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();

            var command = new UpdateProfilePictureCommand
            {
                UserId = userId,
                PictureFile = pictureFile
            };

            return await RequestAsync(command, cancellationToken);
        }
    }
}