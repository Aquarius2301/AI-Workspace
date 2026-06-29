using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Summaries;

public sealed record GetSummaryQuery(
    Guid CurrentUserId,
    CancellationToken CancellationToken = default
) : IRequest<SummaryResponse>;

public sealed record SummaryResponse(
    int TotalTeams,
    int TotalProjects,
    TaskSummary MyTasks,
    List<TaskItemSummary> RecentTasks,
    List<CommentSummary> RecentComments,
    List<TeamSummary> TeamSummaries,
    DateTimeOffset? LastActiveAt
);

public sealed record TaskSummary(
    int Total,
    int Open,
    int InProgress,
    int Done,
    int Blocked,
    int Overdue
);

public sealed record TaskItemSummary(
    Guid Id,
    string Title,
    string ProjectName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DueDate
);

public sealed record CommentSummary(
    Guid Id,
    string Content,
    string? ProjectName,
    DateTimeOffset CreatedAt
);

public sealed record TeamMemberInfo(Guid TeamId, string Role);

public sealed record TeamSummary(
    Guid TeamId,
    string TeamName,
    string MyRole,
    int MemberCount,
    int ProjectCount
);

public sealed class GetSummaryQueryHandler : IRequestHandler<GetSummaryQuery, SummaryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSummaryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SummaryResponse> Handle(
        GetSummaryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = request.CurrentUserId;

        // 1. Get all teams the user is a member of
        var teamMemberships = await _unitOfWork
            .TeamMembers.ReadOnly()
            .Where(tm => tm.UserId == userId)
            .Select(tm => new TeamMemberInfo(tm.TeamId, tm.Role.ToString()))
            .ToListAsync(cancellationToken);

        var teamIds = teamMemberships.Select(tm => tm.TeamId).ToList();
        var totalTeams = teamMemberships.Count;

        // 2. Get total projects the user is a member of
        var totalProjects = await _unitOfWork
            .ProjectMembers.ReadOnly()
            .CountAsync(pm => pm.UserId == userId, cancellationToken);

        // 3. Get tasks assigned to the user
        var now = DateTimeOffset.UtcNow;

        var userTasks = await _unitOfWork
            .TaskItems.ReadOnly()
            .Where(t => t.AssignedToId == userId)
            .Include(t => t.Project)
            .ToListAsync(cancellationToken);

        var taskSummary = new TaskSummary(
            Total: userTasks.Count,
            Open: userTasks.Count(t => t.Status == TaskItemStatus.Open),
            InProgress: userTasks.Count(t => t.Status == TaskItemStatus.InProgress),
            Done: userTasks.Count(t => t.Status == TaskItemStatus.Done),
            Blocked: userTasks.Count(t => t.Status == TaskItemStatus.Blocked),
            Overdue: userTasks.Count(t =>
                t.DueDate.HasValue && t.DueDate.Value < now && t.Status != TaskItemStatus.Done
            )
        );

        var recentTasks = userTasks
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t =>
            {
                var displayStatus = t.Status.ToString();
                if (t.DueDate.HasValue && t.DueDate.Value < now && t.Status != TaskItemStatus.Done)
                {
                    displayStatus = "Overdue";
                }
                return new TaskItemSummary(
                    t.Id,
                    t.Title,
                    t.Project.Name,
                    displayStatus,
                    t.CreatedAt,
                    t.DueDate
                );
            })
            .ToList();

        // 4. Get recent comments by the user and resolve project name
        var recentComments = await GetRecentCommentsAsync(userId, cancellationToken);

        // 5. Get team summaries
        var teamSummaries = await GetTeamSummariesAsync(
            teamIds,
            teamMemberships,
            cancellationToken
        );

        // 6. Get last active time
        var lastActiveAt = await _unitOfWork
            .Users.ReadOnly()
            .Where(u => u.Id == userId)
            .Select(u => u.LastActiveAt)
            .FirstOrDefaultAsync(cancellationToken);

        return new SummaryResponse(
            totalTeams,
            totalProjects,
            taskSummary,
            recentTasks,
            recentComments,
            teamSummaries,
            lastActiveAt
        );
    }

    private async Task<List<CommentSummary>> GetRecentCommentsAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        // Get last 10 comments (we'll filter to 5 after resolving project names)
        var comments = await _unitOfWork
            .Comments.ReadOnly()
            .Where(c => c.CreatorId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        if (comments.Count == 0)
            return new List<CommentSummary>();

        // Resolve project names from references
        var taskItemRefs = comments
            .Where(c => c.ReferenceType == ReferenceType.TaskItem)
            .Select(c => c.ReferenceId)
            .Distinct()
            .ToList();

        var documentRefs = comments
            .Where(c => c.ReferenceType == ReferenceType.Document)
            .Select(c => c.ReferenceId)
            .Distinct()
            .ToList();

        // Get project info for TaskItem references
        Dictionary<Guid, string> taskItemProjectMap = new();
        if (taskItemRefs.Count > 0)
        {
            taskItemProjectMap = await _unitOfWork
                .TaskItems.ReadOnly()
                .Where(t => taskItemRefs.Contains(t.Id))
                .Include(t => t.Project)
                .ToDictionaryAsync(t => t.Id, t => t.Project.Name, cancellationToken);
        }

        // Get project info for Document references
        Dictionary<Guid, string> documentProjectMap = new();
        if (documentRefs.Count > 0)
        {
            documentProjectMap = await _unitOfWork
                .Documents.ReadOnly()
                .Where(d => documentRefs.Contains(d.Id))
                .Include(d => d.Project)
                .ToDictionaryAsync(d => d.Id, d => d.Project.Name, cancellationToken);
        }

        return comments
            .Take(5)
            .Select(c =>
            {
                string? projectName = c.ReferenceType switch
                {
                    ReferenceType.TaskItem => taskItemProjectMap.TryGetValue(
                        c.ReferenceId,
                        out var tn
                    )
                        ? tn
                        : null,
                    ReferenceType.Document => documentProjectMap.TryGetValue(
                        c.ReferenceId,
                        out var dn
                    )
                        ? dn
                        : null,
                    _ => null,
                };

                return new CommentSummary(c.Id, c.Content, projectName, c.CreatedAt);
            })
            .ToList();
    }

    private async Task<List<TeamSummary>> GetTeamSummariesAsync(
        List<Guid> teamIds,
        List<TeamMemberInfo> teamMemberships,
        CancellationToken cancellationToken
    )
    {
        if (teamIds.Count == 0)
            return new List<TeamSummary>();

        // Get member counts per team
        var memberCounts = await _unitOfWork
            .TeamMembers.ReadOnly()
            .Where(tm => teamIds.Contains(tm.TeamId))
            .GroupBy(tm => tm.TeamId)
            .Select(g => new { TeamId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TeamId, x => x.Count, cancellationToken);

        // Get project counts per team
        var projectCounts = await _unitOfWork
            .Projects.ReadOnly()
            .Where(p => teamIds.Contains(p.TeamId))
            .GroupBy(p => p.TeamId)
            .Select(g => new { TeamId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TeamId, x => x.Count, cancellationToken);

        // Get team names
        var teams = await _unitOfWork
            .Teams.ReadOnly()
            .Where(t => teamIds.Contains(t.Id))
            .Select(t => new { t.Id, t.Name })
            .ToListAsync(cancellationToken);

        var teamMap = teams.ToDictionary(t => t.Id, t => t.Name);
        var membershipMap = teamMemberships.ToDictionary(tm => tm.TeamId, tm => tm.Role);

        return teamIds
            .Select(teamId => new TeamSummary(
                teamId,
                teamMap.GetValueOrDefault(teamId, "Unknown"),
                membershipMap.GetValueOrDefault(teamId, "Member"),
                memberCounts.GetValueOrDefault(teamId, 0),
                projectCounts.GetValueOrDefault(teamId, 0)
            ))
            .OrderByDescending(ts => ts.MemberCount)
            .ToList();
    }
}
