using BusinessObject.Enums;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using Infrastructure.Functions.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a paginated list of all users
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns a paginated list of users (id, name, email)</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpGet("")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _mediator.Send(
            new GetUsersQuery(new PaginationRequest(page, pageSize), cancellationToken)
        );
        return Ok(result);
    }

    /// <summary>
    /// Update the current user's profile (name, avatar URL, language)
    /// </summary>
    /// <param name="request">An object containing the fields to update (name, avatarUrl, language)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Success if the profile is updated</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="404">UserNotFound if the user does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPut("")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new UpdateProfileCommand(
                userId,
                request.Name,
                request.AvatarUrl,
                request.Language,
                cancellationToken
            )
        );
        return Ok("Success");
    }

    /// <summary>
    /// Change the current user's password
    /// </summary>
    /// <param name="id">The user ID</param>
    /// <param name="request">An object containing the old and new passwords</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">No Content if password is changed successfully</response>
    /// <response code="400">OldPasswordIncorrect if the old password does not match</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">NoPermissionChangePassword if the user tries to change another user's password</response>
    /// <response code="404">UserNotFound if the user does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPatch("{id:guid}/password")]
    public async Task<IActionResult> ChangePassword(
        Guid id,
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();
        if (userId != id)
            throw new ForbiddenException(ErrorCodes.NoPermissionChangePassword);

        await _mediator.Send(
            new ChangePasswordCommand(
                userId,
                request.OldPassword,
                request.NewPassword,
                cancellationToken
            )
        );
        return NoContent();
    }
}

public sealed record UpdateProfileRequest(
    string? Name,
    string? AvatarUrl,
    LanguageDisplay? Language
);

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword);
