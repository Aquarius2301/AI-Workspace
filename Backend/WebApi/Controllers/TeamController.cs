using BusinessObject.Enums;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using Infrastructure.Functions.Teams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

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
    /// Get a paginated list of teams, optionally filtered by user membership or search term.
    /// Returns team info along with member count and the current user's role in each team.
    /// </summary>
    /// <param name="myTeams">If true, only return teams the current user is a member of</param>
    /// <param name="search">Optional search term to filter by team name</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Response items include: id, name, description, memberCount, currentUserRole.
    /// currentUserRole is null if the user is not a member of the team (when myTeams=false).
    /// </remarks>
    /// <response code="200">Returns a paginated list of teams with member count and user role</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpGet]
    public async Task<IActionResult> GetTeams(
        [FromQuery] bool myTeams = false,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetTeamsQuery(
                userId,
                myTeams,
                search,
                new PaginationRequest(page, pageSize),
                cancellationToken
            )
        );

        return Ok(result);
    }

    /// <summary>
    /// Get team details by ID
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns the team details (id, name, description), or null if not found</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTeam(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTeamQuery(id, cancellationToken));
        return Ok(result);
    }

    /// <summary>
    /// Create a new team. The current user becomes the team Admin.
    /// </summary>
    /// <param name="request">An object containing the team name and optional description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Success if the team is created</response>
    /// <response code="400">TeamNameRequired if team name is missing</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost]
    public async Task<IActionResult> CreateTeam(
        [FromBody] CreateTeamRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException(ErrorCodes.TeamNameRequired);

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new CreateTeamCommand(userId, request.Name, request.Description, cancellationToken)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Update a team's name and/or description. Requires Admin role.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="request">An object containing the updated name and/or description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Success if the team is updated</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">
    /// - Forbidden if the user is not a team Admin
    /// - NoPermissionUpdateTeam if the user lacks permission
    /// </response>
    /// <response code="404">TeamNotFound if the team does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
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
    /// Delete a team and all associated data (members, projects, project members). Requires Admin role.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Success if the team is deleted</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">
    /// - Forbidden if the user is not a team Admin
    /// - NoPermissionDeleteTeam if the user lacks permission
    /// </response>
    /// <response code="404">TeamNotFound if the team does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteTeamCommand(userId, id, cancellationToken));

        return Ok("Success");
    }

    /// <summary>
    /// Get a paginated list of team members, optionally filtered by search term or role
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="search">Optional search term to filter by member name or email</param>
    /// <param name="role">Optional role filter (Admin, Leader, Member)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns a paginated list of team members</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
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
        var result = await _mediator.Send(
            new GetTeamMembersQuery(
                id,
                new PaginationRequest(page, pageSize),
                search,
                role,
                cancellationToken
            )
        );
        return Ok(result);
    }

    /// <summary>
    /// Get the current user's membership details in a team
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns the current user's team membership details</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="404">MemberNotFound if the current user is not a member of the team</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpGet("{id:guid}/me")]
    public async Task<IActionResult> GetTeamMember(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();
        var result = await _mediator.Send(new GetTeamMemberQuery(id, userId, cancellationToken));
        return Ok(result);
    }

    /// <summary>
    /// Add members to a team. Requires Admin role.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="request">An object containing a list of members (userId and optional role)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Existing members are skipped silently.
    /// Non-existent users are skipped silently.
    /// If no role is specified, the member is added as "Member".
    /// </remarks>
    /// <response code="200">Success if members are added</response>
    /// <response code="400">OneMemberRequired if the member list is empty</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">
    /// - Forbidden if the user is not a team Admin
    /// - NoPermissionManageTeam if the user lacks permission
    /// </response>
    /// <response code="404">TeamNotFound if the team does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddTeamMembers(
        Guid id,
        [FromBody] AddTeamMembersRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.Members is null || request.Members.Count == 0)
            throw new BadRequestException(ErrorCodes.OneMemberRequired);

        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(
            new AddTeamMembersCommand(userId, id, request.Members, cancellationToken)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Update a team member's role. Requires Admin role.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="memberId">The member's user ID</param>
    /// <param name="request">An object containing the new role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Ensures at least one Admin remains in the team.
    /// </remarks>
    /// <response code="200">Success if the member's role is updated</response>
    /// <response code="400">
    /// - InvalidRoleRequest if the provided role is not valid
    /// - TeamMinOneAdmin if changing the last Admin to another role
    /// </response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">
    /// - Forbidden if the user is not a team Admin
    /// - NoPermissionUpdateMemberRole if the user lacks permission
    /// </response>
    /// <response code="404">MemberNotFound if the team member does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
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
    /// Remove a member from a team. Requires Admin role.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="memberId">The member's user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Ensures at least one Admin remains in the team.
    /// Also removes the member from all project memberships within the team.
    /// </remarks>
    /// <response code="200">Success if the member is removed</response>
    /// <response code="400">TeamMinOneAdmin if removing the last Admin</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">
    /// - Forbidden if the user is not a team Admin
    /// - NoPermissionRemoveTeamMember if the user lacks permission
    /// </response>
    /// <response code="404">MemberNotFound if the team member does not exist</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> RemoveTeamMember(
        Guid id,
        Guid memberId,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new RemoveTeamMemberCommand(userId, id, memberId, cancellationToken));

        return Ok("Success");
    }

    /// <summary>
    /// Get available users who can be added as team members (users not already in the team)
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 20)</param>
    /// <param name="search">Optional search term to filter by user name or email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns a paginated list of available users</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpGet("{id:guid}/available-members")]
    public async Task<IActionResult> getAvailableMembers(
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
    /// Transfer the Admin role to a CoAdmin. Only the sole Admin can call this.
    /// After transfer, the current Admin becomes CoAdmin and the target becomes Admin.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="targetUserId">The user ID of the CoAdmin to receive the Admin role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Success if the admin role has been transferred</response>
    /// <response code="400">
    /// - TeamMinOneAdminTransferRole if the caller is not the only Admin
    /// - InvalidRoleRequest if the target is not a CoAdmin
    /// </response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">Forbidden if the user is not a team Admin</response>
    /// <response code="404">MemberNotFound if the target is not a team member</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost("{id:guid}/transfer-admin/{targetUserId:guid}")]
    public async Task<IActionResult> TransferAdmin(
        Guid id,
        Guid targetUserId,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new TransferAdminCommand(userId, id, targetUserId, cancellationToken));

        return Ok("Success");
    }

    /// <summary>
    /// Leave a team. The current user removes themselves from the team.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// If the current user is an Admin, ensures at least one Admin remains.
    /// Also removes the user from all project memberships within the team.
    /// </remarks>
    /// <response code="200">Success if the user has left the team</response>
    /// <response code="400">
    /// - NotTeamMember if the user is not a member of the team
    /// - TeamMinOneAdminTransferRole if the user is the last Admin
    /// </response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> LeaveTeam(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new LeaveTeamCommand(userId, id, cancellationToken));

        return Ok("Success");
    }

    /// <summary>
    /// Get a paginated list of projects within a team.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="search">Optional search term to filter by project name</param>
    /// <param name="visibility">Optional visibility filter</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns a paginated list of team projects</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="403">Forbidden if the user is not a member of the team</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpGet("{id:guid}/projects")]
    public async Task<IActionResult> GetTeamProjects(
        Guid id,
        [FromQuery] string? search = null,
        [FromQuery] ProjectVisibility? visibility = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetTeamProjectsQuery(
                userId,
                id,
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

public sealed record AddTeamMembersRequest(List<AddTeamMemberRequest> Members);

public sealed record UpdateMemberRoleRequest(string? Role);
