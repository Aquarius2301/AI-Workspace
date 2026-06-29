using BusinessObject.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Functions.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new project. Requires Admin or Leader role in the team.
    /// </summary>
    /// <param name="request">An object containing teamId, name, optional description, and isPublic flag</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Success if the project is created</response>
    /// <response code="400">NameRequired if name is empty</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">Forbidden if the user is not a team Admin or Leader</response>
    /// <response code="404">TeamNotFound if the team does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException(ErrorCodes.NameRequired);

        var userId = ClaimHelper.GetCurrentUserId();

        var visibility = request.IsPublic is true
            ? ProjectVisibility.Public
            : ProjectVisibility.Private;

        await _mediator.Send(
            new CreateProjectCommand(
                userId,
                request.TeamId,
                request.Name,
                request.Description,
                visibility,
                cancellationToken
            ),
            cancellationToken
        );

        return Ok("Success");
    }
}

public sealed record CreateProjectRequest(
    Guid TeamId,
    string Name,
    string? Description,
    bool? IsPublic
);
