using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetMyTeamsQuery(
    Guid UserId,
    string? Search,
    PaginationRequest Pagination,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<GetMyTeamsResult>>;

public sealed record GetMyTeamsResult(
    Guid Id,
    string Name,
    string? Description,
    string Slug,
    int MemberCount,
    TeamMemberRole CurrentUserRole
);

public sealed class GetMyTeamsQueryHandler
    : IRequestHandler<GetMyTeamsQuery, PaginationResult<GetMyTeamsResult>>
{
    private readonly IAppDbContext _context;

    public GetMyTeamsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<GetMyTeamsResult>> Handle(
        GetMyTeamsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.UserId;
        var search = request.Search;
        var pagination = request.Pagination;

        // Fetch all teams where user is a member, including team members for counting
        var teams = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.UserId == userId)
            .Include(tm => tm.Team)
                .ThenInclude(t => t.TeamMembers)
            .Select(tm => tm.Team)
            .ToListAsync(cancellationToken);

        // Apply search filter using CollationSearchHelper (in-memory because EF Core cannot translate it)
        if (!string.IsNullOrWhiteSpace(search))
        {
            teams = teams
                .Where(t =>
                    CollationSearchHelper.Contains(t.Name, search)
                    || CollationSearchHelper.Contains(t.Description, search)
                )
                .ToList();
        }

        // Get total count after filtering
        var total = teams.Count;

        // Apply pagination
        var pagedTeams = teams
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        // Project to result
        var items = pagedTeams
            .Select(team =>
            {
                var currentUserMembership = team.TeamMembers.First(tm => tm.UserId == userId);

                return new GetMyTeamsResult(
                    team.Id,
                    team.Name,
                    team.Description,
                    team.Slug,
                    team.TeamMembers.Count,
                    currentUserMembership.Role
                );
            })
            .ToList();

        return new PaginationResult<GetMyTeamsResult>(
            pagination.Page,
            pagination.PageSize,
            total,
            items
        );
    }
}
