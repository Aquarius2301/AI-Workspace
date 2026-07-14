using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Features.Users;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkspace.Api.Controllers;

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
    /// Updates the profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// Only the provided fields (Name, FileId, Language) will be updated; null fields are ignored.
    /// The user's LastActiveAt timestamp is also refreshed.
    /// </remarks>
    /// <param name="request">Profile update data. All fields are optional; only non-null values are applied.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Profile updated successfully.</response>
    /// <response code="400">Validation failed (NameRequired, NameMaxLength).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">User not found (UserNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
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
                request.FileId,
                request.Language,
                cancellationToken
            )
        );
        return Ok("Success");
    }

    /// <summary>
    /// Changes the password for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// The old password is verified using PBKDF2 before the new password is hashed and saved.
    /// The new password must be at least 8 characters long, contain at least one uppercase letter,
    /// and at least one special character.
    /// </remarks>
    /// <param name="request">The change password request containing the old and new passwords.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Password changed successfully.</response>
    /// <response code="400">Validation failed: new password does not meet complexity requirements (PasswordRequired, PasswordInvalid).</response>
    /// <response code="401">Old password is incorrect (PasswordInvalid).</response>
    /// <response code="404">User not found (UserNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPatch("password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new ChangePasswordCommand(
                userId,
                request.OldPassword,
                request.NewPassword,
                cancellationToken
            )
        );
        return Ok("Success");
    }
}

public sealed record UpdateProfileRequest(string? Name, string? FileId, LanguageDisplay? Language);

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword);
