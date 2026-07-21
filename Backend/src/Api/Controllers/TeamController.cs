using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Features.Projects;
using AIWorkspace.Application.Features.Teams;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkspace.Api.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public class TeamController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all teams that the current user is a member of.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// Results are paginated and can be filtered by search term (matches team name or description).
    /// Each result includes the user's role within the team and the total member count.
    /// </remarks>
    /// <param name="search">Optional search term to filter teams by name or description.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20).</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a paginated list of teams the user belongs to.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet]
    public async Task<IActionResult> GetMyTeams(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetMyTeamsQuery(
                userId,
                search,
                new PaginationRequest(page, pageSize),
                cancellationToken
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// Retrieves detailed information about a specific team by its ID.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must be a member of the requested team.
    /// Returns team details including the user's role within the team.
    /// </remarks>
    /// <param name="id">The unique identifier of the team.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns team details (GetTeamResult) including Id, Name, Description, and current user's Role.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Team not found or user is not a member (TeamNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTeam(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();
        var result = await _mediator.Send(new GetTeamQuery(id, userId, cancellationToken));
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a team's ID by its slug (URL-friendly name).
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must be a member of the team identified by the slug.
    /// Useful for client-side redirects or resolving a slug to a team ID.
    /// </remarks>
    /// <param name="slug">The URL-friendly slug of the team.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the team's ID (GetIdTeamResult).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Team not found or user is not a member (TeamNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetIdTeamBySlug(
        string slug,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetIdTeamBySlugQuery(slug, userId, cancellationToken)
        );
        return Ok(result);
    }

    /// <summary>
    /// Creates a new team. The creator is automatically assigned the Admin role.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user becomes the Admin of the newly created team.
    /// A unique slug is automatically generated for the team.
    /// </remarks>
    /// <param name="request">Team creation data including name and optional description.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the team's slug (CreateTeamResult) if team created successfully.</response>
    /// <response code="400">Validation failed (TeamNameRequired, TeamNameMaxLength, TeamDescriptionMaxLength).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPost]
    public async Task<IActionResult> CreateTeam(
        [FromBody] CreateTeamRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new CreateTeamCommand(userId, request.Name, request.Description, cancellationToken)
        );

        return Ok(result);
    }

    /// <summary>
    /// Updates the name and/or description of an existing team.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have the <b>Admin</b> role in the team.
    /// Only non-null fields are applied; omitted fields remain unchanged.
    /// </remarks>
    /// <param name="id">The unique identifier of the team to update.</param>
    /// <param name="request">Update data with optional name and description.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Team updated successfully.</response>
    /// <response code="400">Validation failed (TeamNameRequired, TeamNameMaxLength, TeamDescriptionMaxLength).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have the Admin role (Forbidden).</response>
    /// <response code="404">Team not found (NotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTeam(
        Guid id,
        [FromBody] UpdateTeamRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new UpdateTeamCommand(userId, id, request.Name, request.Description, cancellationToken)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Deletes a team and all its associated data.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have the <b>Admin</b> role in the team.
    /// This action is irreversible.
    /// </remarks>
    /// <param name="id">The unique identifier of the team to delete.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Team deleted successfully.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have the Admin role (Forbidden).</response>
    /// <response code="404">Team not found (NotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteTeamCommand(userId, id, cancellationToken));

        return Ok("Success");
    }

    /// <summary>
    /// Retrieves the list of members in a team.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must be a member of the team.
    /// Results are paginated and can be filtered by search term (name or email) and role.
    /// Members are sorted by role hierarchy (Admin first, then CoAdmin, then Member) and alphabetically by name.
    /// </remarks>
    /// <param name="id">The unique identifier of the team.</param>
    /// <param name="search">Optional search term to filter members by name or email.</param>
    /// <param name="role">Optional role filter to show only members with a specific role.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20).</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a paginated list of team members.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Team not found or user is not a member (TeamNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetTeamMembers(
        Guid id,
        [FromQuery] string? search = null,
        [FromQuery] TeamMemberRole? role = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetTeamMembersQuery(
                id,
                userId,
                new PaginationRequest(page, pageSize),
                search,
                role,
                cancellationToken
            )
        );
        return Ok(result);
    }

    /// <summary>
    /// Retrieves users who are not yet members of the team and can be added.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have the <b>Admin</b> or <b>CoAdmin</b> role.
    /// Returns a paginated list of users who are not currently members of the specified team.
    /// </remarks>
    /// <param name="id">The unique identifier of the team.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20).</param>
    /// <param name="search">Optional search term to filter available users by name or email.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a paginated list of available users.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have the Admin or CoAdmin role (Forbidden).</response>
    /// <response code="404">Team not found (TeamNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{id:guid}/available-members")]
    public async Task<IActionResult> GetAvailableMembers(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetAvailableTeamMembersQuery(
                userId,
                id,
                new PaginationRequest(page, pageSize),
                search,
                cancellationToken
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// Adds one or more members to a team.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have the <b>Admin</b> or <b>CoAdmin</b> role.
    /// <b>CoAdmin</b> users can only add new members with the <b>Member</b> role (not Admin or CoAdmin).
    /// Duplicate members (already in the team) are silently skipped.
    /// </remarks>
    /// <param name="id">The unique identifier of the team.</param>
    /// <param name="request">List of members to add, each specifying UserId and optional Role.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Members added successfully (duplicates are skipped silently).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have the required role, or CoAdmin tried to assign a non-Member role (Forbidden).</response>
    /// <response code="404">Team not found (TeamNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddTeamMembers(
        Guid id,
        [FromBody] AddTeamMembersRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new AddTeamMembersCommand(
                userId,
                id,
                request.Members.Select(m => new AddTeamMemberItem(m.UserId, m.Role)).ToList(),
                cancellationToken
            )
        );

        return Ok("Success");
    }

    /// <summary>
    /// Updates the role of a specific team member.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must have the <b>Admin</b> role in the team.
    /// Only non-null role values are applied; if Role is null, the member's role remains unchanged.
    /// </remarks>
    /// <param name="id">The unique identifier of the team.</param>
    /// <param name="memberId">The unique identifier of the team member to update.</param>
    /// <param name="request">Update data with the new role.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Member role updated successfully.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have the Admin role (Forbidden).</response>
    /// <response code="404">Team or member not found (UserNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPut("{id:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> UpdateTeamMemberRole(
        Guid id,
        Guid memberId,
        [FromBody] UpdateMemberRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new UpdateTeamMemberRoleCommand(userId, id, memberId, request.Role, cancellationToken)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Removes a member from the team, deletes their project memberships, and unassigns their tasks.
    /// </summary>
    /// <remarks>
    /// This action requires authentication.
    /// <b>Admin</b> can remove Member or CoAdmin (but not another Admin).
    /// <b>CoAdmin</b> can only remove Member.
    /// Users cannot remove themselves.
    /// </remarks>
    /// <param name="id">The unique identifier of the team.</param>
    /// <param name="memberId">The unique identifier of the member to remove.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Member removed successfully.</response>
    /// <response code="400">User tried to remove themselves (BadRequest).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="403">User does not have permission (Forbidden).</response>
    /// <response code="404">Team member not found (UserNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> RemoveTeamMember(
        Guid id,
        Guid memberId,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new RemoveTeamMemberCommand(userId, id, memberId, cancellationToken)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Retrieves all projects belonging to a specific team.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user must be a member of the team.
    /// <b>Admin</b> and <b>CoAdmin</b> members can view all projects in the team.
    /// Regular <b>Member</b> users can only view public projects or projects where they are a project member.
    /// The result includes a <c>CanView</c> flag indicating whether the user has access to each project.
    /// Results are paginated and can be filtered by search term and visibility.
    /// </remarks>
    /// <param name="teamId">The unique identifier of the team.</param>
    /// <param name="search">Optional search term to filter projects by name.</param>
    /// <param name="visibility">Optional filter by project visibility (Public or Private).</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 20).</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a paginated list of projects.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">Team not found or user is not a member (TeamNotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpGet("{teamId:guid}/projects")]
    public async Task<IActionResult> GetProjectsByTeam(
        Guid teamId,
        [FromQuery] string? search = null,
        [FromQuery] ProjectVisibility? visibility = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetProjectsByTeamQuery(
                userId,
                teamId,
                search,
                visibility,
                new PaginationRequest(page, pageSize),
                cancellationToken
            )
        );

        return Ok(result);
    }
}

public sealed record CreateTeamRequest(string Name, string? Description);

public sealed record UpdateTeamRequest(string? Name, string? Description);

public sealed record UpdateMemberRoleRequest(TeamMemberRole? Role);

public sealed record AddTeamMembersRequest(List<AddTeamMemberRequestItem> Members);

public sealed record AddTeamMemberRequestItem(Guid UserId, TeamMemberRole? Role);
