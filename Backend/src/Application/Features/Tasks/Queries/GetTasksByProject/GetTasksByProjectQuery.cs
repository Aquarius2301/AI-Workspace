using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

public sealed record GetTasksByProjectQuery(
    Guid CurrentUserId,
    Guid ProjectId,
    string? Search,
    TaskPriority? Priority,
    Guid? AssigneeId,
    CancellationToken CancellationToken
) : IRequest<List<TaskItemResult>>;

public sealed class GetTasksByProjectQueryHandler
    : IRequestHandler<GetTasksByProjectQuery, List<TaskItemResult>>
{
    // Maximum tasks returned for Kanban view to prevent unbounded loading
    private const int MaxTaskLimit = 200;

    private readonly IAppDbContext _context;

    public GetTasksByProjectQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItemResult>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken
    )
    {
        var projectId = request.ProjectId;
        var currentUserId = request.CurrentUserId;

        // Verify the project exists and user has access
        var project =
            await _context
                .Projects.AsNoTracking()
                .Where(p => p.Id == projectId)
                .Select(p => new { p.TeamId })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.NotFound);

        var isTeamAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == currentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        var isProjectMember = await _context
            .ProjectMembers.AsNoTracking()
            .AnyAsync(
                pm => pm.ProjectId == projectId && pm.UserId == currentUserId,
                cancellationToken
            );

        if (!isTeamAdminOrCoAdmin && !isProjectMember)
        {
            throw new NotFoundException(ErrorCodes.NotFound);
        }

        // Build query at DB level — all filters pushed to SQL
        var query = _context.TaskItems.AsNoTracking().Where(t => t.ProjectId == projectId);

        // Apply search filter at DB level using SQL Server collation
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(t =>
                EF.Functions.Collate(t.Title, "SQL_Latin1_General_CP1_CI_AI")
                    .Contains(request.Search)
                || EF.Functions.Collate(t.Description ?? "", "SQL_Latin1_General_CP1_CI_AI")
                    .Contains(request.Search)
            );
        }

        // Apply priority filter at DB level if provided
        if (request.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == request.Priority.Value);
        }

        // Apply assignee filter at DB level if provided
        if (request.AssigneeId.HasValue)
        {
            query = query.Where(t => t.AssignedToId == request.AssigneeId.Value);
        }

        // Single round-trip: filter + sort + limit at DB, projection via Select
        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Take(MaxTaskLimit)
            .Select(t => new TaskItemResult(
                t.Id,
                t.ProjectId,
                t.Project.Name,
                t.Title,
                t.Description,
                t.AssignedToId,
                t.AssignedTo != null ? t.AssignedTo.Name : null,
                t.Priority,
                t.Status,
                t.CreatedAt,
                t.DueDate
            ))
            .ToListAsync(cancellationToken);
    }
}
