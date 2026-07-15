using AIWorkspace.Application.Common;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Summaries;

public sealed record GetSummaryQuery(
    Guid CurrentUserId,
    CancellationToken CancellationToken = default
) : IRequest<SummaryResponse>;

public sealed record SummaryResponse(
    int TotalTeams,
    int TotalProjects,
    TaskSummary MyTasks,
    List<TaskItemSummary> RecentTasks,
    List<TeamSummary> TeamSummaries
);

public sealed record TaskSummary(int Total, int ToDo, int Doing, int Done, int Overdue);

public sealed record TaskItemSummary(
    Guid Id,
    string Title,
    Guid ProjectId,
    string ProjectName,
    string ProjectSlug,
    TaskItemStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DueDate
);

public sealed record TeamSummary(
    Guid TeamId,
    string TeamName,
    string Slug,
    TeamMemberRole MyRole,
    int MemberCount,
    int ProjectCount
);

public sealed class GetSummaryQueryHandler : IRequestHandler<GetSummaryQuery, SummaryResponse>
{
    private readonly IAppDbContext _context;

    public GetSummaryQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<SummaryResponse> Handle(
        GetSummaryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.CurrentUserId;
        var now = DateTimeOffset.UtcNow;

        // Get teams where user is a member + team data in one query
        var teamMemberships = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => tm.UserId == userId)
            .Include(tm => tm.Team)
            .ToListAsync(cancellationToken);

        var teamIds = teamMemberships.Select(tm => tm.TeamId).ToList();
        var totalTeams = teamMemberships.Count;

        // Get project counts per team (DB-side aggregation)
        var teamProjectCounts = await _context
            .Projects.AsNoTracking()
            .Where(p => teamIds.Contains(p.TeamId))
            .GroupBy(p => p.TeamId)
            .Select(g => new { TeamId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TeamId, x => x.Count, cancellationToken);

        var totalProjects = teamProjectCounts.Values.Sum();

        // Get member counts per team (DB-side aggregation)
        var teamMemberCounts = await _context
            .TeamMembers.AsNoTracking()
            .Where(tm => teamIds.Contains(tm.TeamId))
            .GroupBy(tm => tm.TeamId)
            .Select(g => new { TeamId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TeamId, x => x.Count, cancellationToken);

        // Task counts using DB-side aggregation - avoids loading all tasks into memory
        var taskCounts = await _context
            .TaskItems.AsNoTracking()
            .Where(t => t.AssignedToId == userId)
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var todo = taskCounts.FirstOrDefault(x => x.Status == TaskItemStatus.ToDo)?.Count ?? 0;
        var doing = taskCounts.FirstOrDefault(x => x.Status == TaskItemStatus.Doing)?.Count ?? 0;
        var done = taskCounts.FirstOrDefault(x => x.Status == TaskItemStatus.Done)?.Count ?? 0;
        var total = taskCounts.Sum(x => x.Count);

        // Overdue count: tasks not Done with DueDate < now
        var overdue = await _context
            .TaskItems.AsNoTracking()
            .CountAsync(
                t =>
                    t.AssignedToId == userId
                    && t.Status != TaskItemStatus.Done
                    && t.DueDate != null
                    && t.DueDate < now,
                cancellationToken
            );

        // Recent 10 tasks - load only needed fields, avoid materializing all tasks
        var recentTasks = await _context
            .TaskItems.AsNoTracking()
            .Where(t => t.AssignedToId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .Select(t => new TaskItemSummary(
                t.Id,
                t.Title,
                t.ProjectId,
                t.Project.Name,
                t.Project.Slug,
                t.Status,
                t.CreatedAt,
                t.DueDate
            ))
            .ToListAsync(cancellationToken);

        // Build team summaries
        var teamSummaries = teamMemberships
            .Select(tm => new TeamSummary(
                tm.TeamId,
                tm.Team.Name,
                tm.Team.Slug,
                tm.Role,
                teamMemberCounts.GetValueOrDefault(tm.TeamId, 0),
                teamProjectCounts.GetValueOrDefault(tm.TeamId, 0)
            ))
            .ToList();

        return new SummaryResponse(
            totalTeams,
            totalProjects,
            new TaskSummary(total, todo, doing, done, overdue),
            recentTasks,
            teamSummaries
        );
    }
}
