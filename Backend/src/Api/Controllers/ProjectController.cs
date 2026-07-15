using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Features.Projects;
using AIWorkspace.Application.Features.Tasks;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkspace.Api.Controllers;

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
    /// Retrieves all projects that the current user is a member of.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// Only projects where the user is listed as a project member are returned.
    /// Results are paginated and can be filtered by search term and visibility.
    /// Each result includes team name, user role, member count, and task completion stats.
    /// </remarks>
    /// <param name="search">Optional search term to filter projects by name or description.</param>
    /// <param name="visibility">Optional filter by project visibility (Public or Private).</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20).</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a paginated list of projects the user is a member of.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet]
    public async Task<IActionResult> GetMyProjects(
        [FromQuery] string? search = null,
        [FromQuery] ProjectVisibility? visibility = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetMyProjectsQuery(
                userId,
                search,
                visibility,
                new PaginationRequest(page, pageSize),
                cancellationToken
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// Retrieves detailed information about a specific project by its ID.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must be a member of the project's team.
    /// <b>Admin</b> and <b>CoAdmin</b> team members can view all projects.
    /// Regular team members can only view public projects or projects they are assigned to.
    /// If the project is private and the user lacks access, a 404 is returned.
    /// </remarks>
    /// <param name="id">The unique identifier of the project.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns project details (GetProjectResult).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Project not found or user does not have access (NotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProject(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetProjectQuery(id, userId, cancellationToken));

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a project's ID by its slug (URL-friendly name).
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have access to the project:
    /// they must be either a <b>team Admin/CoAdmin</b> or a <b>project member</b>.
    /// Useful for client-side redirects or resolving a slug to a project ID.
    /// </remarks>
    /// <param name="slug">The URL-friendly slug of the project.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the project's ID (GetIdProjectResult).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Project not found or user does not have access (NotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetIdProjectBySlug(
        string slug,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetIdProjectBySlugQuery(slug, userId, cancellationToken)
        );

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the list of members in a project.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have access to the project:
    /// they must be either a <b>team Admin/CoAdmin</b> or a <b>project member</b>.
    /// Results are paginated and can be filtered by search term (name or email) and role.
    /// Members are sorted by join date.
    /// </remarks>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="search">Optional search term to filter members by name or email.</param>
    /// <param name="role">Optional role filter to show only members with a specific role.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20).</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a paginated list of project members.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Project not found (ProjectNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{projectId:guid}/members")]
    public async Task<IActionResult> GetProjectMembers(
        Guid projectId,
        [FromQuery] string? search = null,
        [FromQuery] ProjectRole? role = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetProjectMembersQuery(
                userId,
                projectId,
                new PaginationRequest(page, pageSize),
                search,
                role,
                cancellationToken
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// Creates a new project within a team.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have the <b>Admin</b> or <b>CoAdmin</b> role in the team.
    /// A unique slug is automatically generated for the project.
    /// The project name is required and validated by the controller.
    /// </remarks>
    /// <param name="request">Project creation data including team ID, name, optional description, and visibility.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the project's slug (CreateTeamResult) if project created successfully.</response>
    /// <response code="400">Project name is required (ProjectNameRequired).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have the Admin or CoAdmin role in the team (Forbidden).</response>
    /// <response code="404">Team not found (TeamNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPost]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException(ErrorCodes.ProjectNameRequired);

        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new CreateProjectCommand(
                userId,
                request.TeamId,
                request.Name,
                request.Description,
                request.Visibility,
                cancellationToken
            ),
            cancellationToken
        );

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing project's name, description, or visibility.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have the <b>Admin</b> or <b>CoAdmin</b> role in the project's team.
    /// Only provided fields will be updated (partial update).
    /// </remarks>
    /// <param name="id">The unique identifier of the project to update.</param>
    /// <param name="request">Update data including optional name, description, and visibility.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Project updated successfully.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have the Admin or CoAdmin role in the team (Forbidden).</response>
    /// <response code="404">Project not found (ProjectNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new UpdateProjectCommand(
                userId,
                id,
                request.Name,
                request.Description,
                request.Visibility,
                cancellationToken
            ),
            cancellationToken
        );

        return Ok("Success");
    }

    /// <summary>
    /// Retrieves tasks assigned to the current user within a specific project.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have access to the project:
    /// they must be either a <b>team Admin/CoAdmin</b> or a <b>project member</b>.
    /// Results are paginated and can be filtered by search term, status, and priority.
    /// Only tasks assigned to the current user are returned.
    /// </remarks>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="search">Optional search term to filter tasks by title or description.</param>
    /// <param name="status">Optional filter by task status.</param>
    /// <param name="priority">Optional filter by task priority.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20).</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a paginated list of tasks assigned to the current user.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Project not found or user does not have access (NotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{projectId:guid}/tasks/me")]
    public async Task<IActionResult> GetMyTasksByProject(
        Guid projectId,
        [FromQuery] string? search = null,
        [FromQuery] TaskItemStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetMyTasksByProjectQuery(
                userId,
                projectId,
                search,
                status,
                priority,
                new PaginationRequest(page, pageSize),
                cancellationToken
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all tasks within a specific project.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have access to the project:
    /// they must be either a <b>team Admin/CoAdmin</b> or a <b>project member</b>.
    /// Results can be filtered by search term, priority, and assignee.
    /// Unlike the "tasks/me" endpoint, this returns all tasks (not just the user's own).
    /// Returns a flat list (no pagination), ordered by creation date descending.
    /// </remarks>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="search">Optional search term to filter tasks by title or description.</param>
    /// <param name="priority">Optional filter by task priority.</param>
    /// <param name="assigneeId">Optional filter by assignee user ID.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a list of tasks in the project.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Project not found or user does not have access (NotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{projectId:guid}/tasks")]
    public async Task<IActionResult> GetTasksByProject(
        Guid projectId,
        [FromQuery] string? search = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] Guid? assigneeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetTasksByProjectQuery(
                userId,
                projectId,
                search,
                priority,
                assigneeId,
                cancellationToken
            )
        );

        return Ok(result);
    }
}

public sealed record CreateProjectRequest(
    Guid TeamId,
    string Name,
    string? Description,
    ProjectVisibility Visibility
);

public sealed record UpdateProjectRequest(
    string? Name,
    string? Description,
    ProjectVisibility? Visibility
);
