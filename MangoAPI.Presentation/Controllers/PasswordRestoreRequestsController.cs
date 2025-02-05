﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MangoAPI.Application.Interfaces;
using MangoAPI.BusinessLogic.ApiCommands.PasswordRestoreRequests;
using MangoAPI.BusinessLogic.Responses;
using MangoAPI.Presentation.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MangoAPI.Presentation.Controllers;

/// <summary>
/// Controller responsible for Password Restore Request Entity.
/// </summary>
[ApiController]
[Route("api/password-restore-request")]
[Authorize]
public class PasswordRestoreRequestsController : ApiControllerBase, IPasswordRestoreRequestsController
{
    public PasswordRestoreRequestsController(IMediator mediator, IMapper mapper, ICorrelationContext correlationContext)
        : base(mediator, mapper, correlationContext)
    {
    }

    /// <summary>
    /// Creates new password restore request in database.
    /// </summary>
    /// <param name="email">Email or phone of user.</param>
    /// <param name="cancellationToken">Cancellation token instance.</param>
    [AllowAnonymous]
    [HttpPost]
    [SwaggerOperation(
        Description = "Creates new password restore request in database. Request valid for 3 hours.",
        Summary = "Creates new password restore request in database.")]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RestorePasswordRequestAsync(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        var command = new RequestPasswordRestoreCommand(email);

        return await RequestAsync(command, cancellationToken);
    }

    /// <summary>
    /// Updates users password hash in database.
    /// </summary>
    /// <param name="request">Request instance.</param>
    /// <param name="cancellationToken">Cancellation token instance.</param>
    /// <returns>Possible codes: 200, 400, 409.</returns>
    [HttpPut]
    [AllowAnonymous]
    [SwaggerOperation(
        Description = "Updates users password hash in database.",
        Summary = "Updates users password hash in database.")]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RestorePasswordAsync(
        [FromBody] PasswordRestoreRequest request,
        CancellationToken cancellationToken)
    {
        var command = Mapper.Map<PasswordRestoreCommand>(request);

        return await RequestAsync(command, cancellationToken);
    }
}
