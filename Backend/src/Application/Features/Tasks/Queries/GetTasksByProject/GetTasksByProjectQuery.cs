using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

/// <summary>
/// Query to get all tasks within a specific project.
/// The requesting user must be a project member or a team Admin/CoAdmin.
/// </summary>
/// <param name="CurrentUserId">The ID of the authenticated user</param>
/// <param name="ProjectId">The ID of the project</param>
/// <param name="Search">Optional search term to filter by task title or description</param>
/// <param name="Priority">Optional priority filter</param>
/// <param name="AssigneeId">Optional assignee filter</param>
/// <param name="CancellationToken">Cancellation token</param>
public sealed record GetTasksByProjectQuery(
    Guid CurrentUserId,
    Guid ProjectId,
    string? Search,
    TaskPriority? Priority,
    Guid? AssigneeId,
    CancellationToken CancellationToken
) : IRequest<List<TaskItemResult>>;

/// <summary>
/// Handler for <see cref="GetTasksByProjectQuery"/>.
/// Returns all tasks for a project with optional filtering by search, priority, and assignee.
/// Search uses CollationSearchHelper for Vietnamese diacritics-insensitive matching.
/// </summary>
public sealed class GetTasksByProjectQueryHandler
    : IRequestHandler<GetTasksByProjectQuery, List<TaskItemResult>>
{
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

        // Build query for all tasks in this project
        var query = _context
            .TaskItems.AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .AsQueryable();

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

        // Materialize to memory for CollationSearchHelper (cannot translate to SQL)
        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);

        // Apply search filter in-memory using Vietnamese diacritics-insensitive matching
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            tasks = tasks
                .Where(t =>
                    CollationSearchHelper.Contains(t.Title, request.Search)
                    || CollationSearchHelper.Contains(t.Description, request.Search)
                )
                .ToList();
        }

        return tasks
            .Select(t => new TaskItemResult(
                t.Id,
                t.ProjectId,
                t.Project.Name,
                t.Title,
                t.Description,
                t.AssignedToId,
                t.AssignedTo?.Name,
                t.Priority,
                t.Status,
                t.CreatedAt,
                t.DueDate
            ))
            .ToList();
    }
}
