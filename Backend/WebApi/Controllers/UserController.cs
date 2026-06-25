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

    [HttpPut("")]
    public async Task<IActionResult> UpdateProfile(
        Guid id,
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

    [HttpPatch("{id:guid}/password")]
    public async Task<IActionResult> ChangePassword(
        Guid id,
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();
        if (userId != id)
            throw new ForbiddenException("You can only change your own password");

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
