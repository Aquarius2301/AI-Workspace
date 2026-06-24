using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Queries;

public sealed record GetProjectTasksQuery(
    Guid CurrentUserId,
    Guid ProjectId,
    TaskItemStatus? Status,
    Guid? MemberId,
    PaginationRequest Pagination
) : IRequest<PaginationResult<TaskItemResponse>>;

public sealed record TaskItemResponse(
    Guid Id,
    string Title,
    string? Description,
    Guid? AssignedToId,
    string? AssignedToName,
    int Priority,
    string Status,
    DateTime CreatedAt,
    DateTime? DueDate
);

public sealed class GetProjectTasksQueryHandler
    : IRequestHandler<GetProjectTasksQuery, PaginationResult<TaskItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectTasksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<TaskItemResponse>> Handle(
        GetProjectTasksQuery request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _unitOfWork
                .Projects.ReadOnly()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Check user is a team member
        var isTeamMember = await _unitOfWork
            .TeamMembers.ReadOnly()
            .AnyAsync(
                tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException(ErrorCodes.NotTeamMember);

        // If private project, check user is project member unless Admin/Leader
        if (project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.ReadOnly()
                .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
                .Select(tm => tm.Role)
                .FirstOrDefaultAsync(cancellationToken);

            var isAdminOrLeader =
                teamRole == TeamMemberRole.Admin || teamRole == TeamMemberRole.Leader;

            if (!isAdminOrLeader)
            {
                var isProjectMember = await _unitOfWork
                    .ProjectMembers.ReadOnly()
                    .AnyAsync(
                        pm =>
                            pm.ProjectId == request.ProjectId && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException(ErrorCodes.NotPrivateProjectMember);
            }
        }

        // Build query
        var query = _unitOfWork
            .TaskItems.ReadOnly()
            .Include(t => t.AssignedTo)
            .Where(t => t.ProjectId == request.ProjectId);

        // Filter by status
        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        // Filter by member
        if (request.MemberId.HasValue && request.MemberId.Value != Guid.Empty)
        {
            query = query.Where(t => t.AssignedToId == request.MemberId.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(t => new TaskItemResponse(
                t.Id,
                t.Title,
                t.Description,
                t.AssignedToId,
                t.AssignedTo != null ? t.AssignedTo.Name : null,
                t.Priority,
                t.Status.ToString(),
                t.CreatedAt,
                t.DueDate
            ))
            .ToListAsync(cancellationToken);

        return new PaginationResult<TaskItemResponse>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            items
        );
    }
}
