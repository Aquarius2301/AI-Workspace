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
        [FromQuery] string? sortBy = null,
        [FromQuery] bool? sortDesc = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new GetTeamsQuery(userId, myTeams, search, new PaginationRequest(page, pageSize))
        );

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTeam(Guid id)
    {
        var result = await _mediator.Send(new GetTeamQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Team name is required");

        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new CreateTeamCommand(userId, request.Name, request.Description)
        );

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamRequest request)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new UpdateTeamCommand(userId, id, request.Name, request.Description)
        );

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new DeleteTeamCommand(userId, id));

        return Ok("Success");
    }

    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetTeamMembers(Guid id)
    {
        var result = await _mediator.Send(new GetTeamMembersQuery(id));
        return Ok(result);
    }

    [HttpGet("{id:guid}/me")]
    public async Task<IActionResult> GetTeamMember(Guid id)
    {
        var userId = ClaimHelper.GetCurrentUserId();
        var result = await _mediator.Send(new GetTeamMemberQuery(id, userId));
        return Ok(result);
    }

    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddTeamMembers(
        Guid id,
        [FromBody] AddTeamMembersRequest request
    )
    {
        if (request.Members is null || request.Members.Count == 0)
            throw new BadRequestException("At least one member is required");

        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new AddTeamMembersCommand(userId, id, request.Members));

        return Ok(result);
    }

    [HttpPut("{id:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> UpdateTeamMemberRole(
        Guid id,
        Guid memberId,
        [FromBody] UpdateMemberRoleRequest request
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(
            new UpdateTeamMemberRoleCommand(userId, id, memberId, request.Role)
        );

        return Ok(result);
    }

    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> RemoveTeamMember(Guid id, Guid memberId)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new RemoveTeamMemberCommand(userId, id, memberId));

        return Ok("Success");
    }

    [HttpGet("available-members")]
    public async Task<IActionResult> GetAvailableMembers()
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetGlobalAvailableMembersQuery(userId));

        return Ok(result);
    }

    [HttpGet("{id:guid}/available-members")]
    public async Task<IActionResult> GetAvailableTeamMembers(Guid id)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new GetAvailableTeamMembersQuery(userId, id));

        return Ok(result);
    }

    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> LeaveTeam(Guid id)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new LeaveTeamCommand(userId, id));

        return Ok("Success");
    }
}

public sealed record CreateTeamRequest(string Name, string? Description);

public sealed record UpdateTeamRequest(string? Name, string? Description);

public sealed record AddTeamMembersRequest(List<AddTeamMemberRequest> Members);

public sealed record UpdateMemberRoleRequest(string? Role);
