using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Models;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

public sealed record TaskItemResult(
    Guid Id,
    Guid ProjectId,
    string ProjectName,
    string Title,
    string? Description,
    Guid? AssignedToId,
    string? AssignedToName,
    TaskPriority Priority,
    TaskItemStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DueDate
);

public sealed record GetMyTasksByProjectQuery(
    Guid CurrentUserId,
    Guid ProjectId,
    string? Search,
    TaskItemStatus? Status,
    TaskPriority? Priority,
    PaginationRequest Pagination,
    CancellationToken CancellationToken
) : IRequest<PaginationResult<TaskItemResult>>;

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
            .Where(t => t.ProjectId == projectId && t.AssignedToId == currentUserId);

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

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
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

        return new PaginationResult<TaskItemResult>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            totalCount,
            items
        );
    }
}
