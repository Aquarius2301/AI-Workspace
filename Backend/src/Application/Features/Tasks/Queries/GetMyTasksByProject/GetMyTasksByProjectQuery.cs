using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

/// <summary>
/// Result item containing task information.
/// </summary>
/// <param name="Id">The unique identifier of the task</param>
/// <param name="ProjectId">The unique identifier of the project</param>
/// <param name="ProjectName">The name of the project</param>
/// <param name="Title">The title of the task</param>
/// <param name="Description">The optional description of the task</param>
/// <param name="AssignedToId">The optional ID of the assigned user</param>
/// <param name="AssignedToName">The optional name of the assigned user</param>
/// <param name="Priority">The priority of the task</param>
/// <param name="Status">The current status of the task</param>
/// <param name="CreatedAt">The date and time when the task was created</param>
/// <param name="DueDate">The optional due date of the task</param>
public sealed record TaskItemResult(
    Guid Id,
    Guid ProjectId,
    string ProjectName,
    string Title,
    string? Description,
    Guid? AssignedToId,
    string? AssignedToName,
    string Priority,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DueDate
);

/// <summary>
/// Query to get a paginated list of tasks assigned to the current user within a specific project.
/// </summary>
/// <param name="CurrentUserId">The ID of the authenticated user</param>
/// <param name="ProjectId">The ID of the project</param>
/// <param name="Search">Optional search term to filter by task title or description</param>
/// <param name="Status">Optional status filter</param>
/// <param name="Priority">Optional priority filter</param>
/// <param name="Pagination">Pagination parameters</param>
/// <param name="CancellationToken">Cancellation token</param>
public sealed record GetMyTasksByProjectQuery(
    Guid CurrentUserId,
    Guid ProjectId,
    string? Search,
    TaskItemStatus? Status,
    TaskPriority? Priority,
    PaginationRequest Pagination,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<TaskItemResult>>;

/// <summary>
/// Handler for <see cref="GetMyTasksByProjectQuery"/>.
/// Returns tasks assigned to the current user within a project,
/// with optional filtering by search, status, and priority.
/// </summary>
public sealed class GetMyTasksByProjectQueryHandler
    : IRequestHandler<GetMyTasksByProjectQuery, PaginationResult<TaskItemResult>>
{
    private readonly IAppDbContext _context;

    public GetMyTasksByProjectQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<TaskItemResult>> Handle(
        GetMyTasksByProjectQuery request,
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

        // Build query for tasks assigned to current user in this project
        var query = _context
            .TaskItems.AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.AssignedToId == currentUserId)
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .AsQueryable();

        // Apply search filter in-memory using Vietnamese diacritics-insensitive matching
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var tasks = await query.ToListAsync(cancellationToken);

            tasks = tasks
                .Where(t =>
                    CollationSearchHelper.Contains(t.Title, request.Search)
                    || CollationSearchHelper.Contains(t.Description, request.Search)
                )
                .ToList();

            var total = tasks.Count;

            var pagedTasks = tasks
                .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize)
                .ToList();

            var items = pagedTasks.Select(MapToResult).ToList();

            return new PaginationResult<TaskItemResult>(
                request.Pagination.Page,
                request.Pagination.PageSize,
                total,
                items
            );
        }

        // Apply status filter at DB level if provided
        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        // Apply priority filter at DB level if provided
        if (request.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == request.Priority.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var taskList = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        var resultItems = taskList.Select(MapToResult).ToList();

        return new PaginationResult<TaskItemResult>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            totalCount,
            resultItems
        );
    }

    private static TaskItemResult MapToResult(Domain.Entities.TaskItem task)
    {
        return new TaskItemResult(
            task.Id,
            task.ProjectId,
            task.Project.Name,
            task.Title,
            task.Description,
            task.AssignedToId,
            task.AssignedTo?.Name,
            task.Priority.ToString(),
            task.Status.ToString(),
            task.CreatedAt,
            task.DueDate
        );
    }
}
