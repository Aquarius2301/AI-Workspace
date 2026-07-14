using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetAvailableTeamMembersQuery(
    Guid UserId,
    Guid TeamId,
    PaginationRequest Pagination,
    string? Search,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<GetAvailableTeamMembersResult>>, IRequireTeamRole
{
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => UserId;
    TeamMemberRole[] IRequireTeamRole.AllowedRoles =>
        [TeamMemberRole.Admin, TeamMemberRole.CoAdmin];
}

public sealed record GetAvailableTeamMembersResult(
    Guid Id,
    string Name,
    string Email,
    string? AvatarUrl
);

public sealed class GetAvailableTeamMembersQueryHandler
    : IRequestHandler<GetAvailableTeamMembersQuery, PaginationResult<GetAvailableTeamMembersResult>>
{
    private readonly IAppDbContext _context;

    public GetAvailableTeamMembersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<GetAvailableTeamMembersResult>> Handle(
        GetAvailableTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var teamId = request.TeamId;
        var search = request.Search;
        var pagination = request.Pagination;

        // Role authorization is handled by TeamRoleBehavior (Admin/CoAdmin only)

        // Get all users who are NOT members of this team
        var availableUsers = await _context
            .Users.AsNoTracking()
            .Where(u => !_context.TeamMembers.Any(tm => tm.TeamId == teamId && tm.UserId == u.Id))
            .ToListAsync(cancellationToken);

        // Apply search filter using CollationSearchHelper (in-memory)
        if (!string.IsNullOrWhiteSpace(search))
        {
            availableUsers = availableUsers
                .Where(u =>
                    CollationSearchHelper.Contains(u.Name, search)
                    || CollationSearchHelper.Contains(u.Email, search)
                )
                .ToList();
        }

        // Get total count after filtering
        var total = availableUsers.Count;

        // Apply pagination
        var pagedUsers = availableUsers
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        // Project to result
        var items = pagedUsers
            .Select(u => new GetAvailableTeamMembersResult(
                u.Id,
                u.Name,
                u.Email,
                u.AvatarPicture?.Url
            ))
            .ToList();

        return new PaginationResult<GetAvailableTeamMembersResult>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
