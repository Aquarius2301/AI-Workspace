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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTeam(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTeamQuery(id, cancellationToken));
        return Ok(result);
    }

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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteTeamCommand(userId, id, cancellationToken));

        return Ok("Success");
    }

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

    [HttpGet("{id:guid}/me")]
    public async Task<IActionResult> GetTeamMember(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();
        var result = await _mediator.Send(new GetTeamMemberQuery(id, userId, cancellationToken));
        return Ok(result);
    }

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

    [HttpGet("{id:guid}/available-members")]
    public async Task<IActionResult> GetAvailableTeamMembers(
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

    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> LeaveTeam(Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new LeaveTeamCommand(userId, id, cancellationToken));

        return Ok("Success");
    }
}

public sealed record CreateTeamRequest(string Name, string? Description);

public sealed record UpdateTeamRequest(string? Name, string? Description);

public sealed record AddTeamMembersRequest(List<AddTeamMemberRequest> Members);

public sealed record UpdateMemberRoleRequest(string? Role);
