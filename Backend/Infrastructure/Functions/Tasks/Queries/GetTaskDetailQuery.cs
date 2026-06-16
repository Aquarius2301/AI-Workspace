using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Queries;

public sealed record GetTaskDetailQuery(Guid CurrentUserId, Guid TaskId)
    : IRequest<TaskDetailResponse>;

public sealed record TaskDetailResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssignedToId,
    string? AssignedToName,
    int Priority,
    string Status,
    DateTime CreatedAt,
    DateTime? DueDate
);

public sealed class GetTaskDetailQueryHandler
    : IRequestHandler<GetTaskDetailQuery, TaskDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskDetailQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDetailResponse> Handle(
        GetTaskDetailQuery request,
        CancellationToken cancellationToken
    )
    {
        var task =
            await _unitOfWork
                .TaskItems.GetQuery()
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task not found");

        // Check user is a member of the project's team
        var isTeamMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new ForbiddenException("You are not a member of this team");

        // If private project, check user is project member unless Admin/Leader
        if (task.Project.Visibility == ProjectVisibility.Private)
        {
            var teamRole = await _unitOfWork
                .TeamMembers.GetQuery()
                .Where(tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId)
                .Select(tm => tm.Role)
                .FirstOrDefaultAsync(cancellationToken);

            var isAdminOrLeader =
                teamRole == TeamMemberRole.Admin || teamRole == TeamMemberRole.Leader;

            if (!isAdminOrLeader)
            {
                var isProjectMember = await _unitOfWork
                    .ProjectMembers.GetQuery()
                    .AnyAsync(
                        pm => pm.ProjectId == task.ProjectId && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!isProjectMember)
                    throw new ForbiddenException("You are not a member of this private project");
            }
        }

        return new TaskDetailResponse(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.AssignedToId,
            task.AssignedTo?.Name,
            task.Priority,
            task.Status.ToString(),
            task.CreatedAt,
            task.DueDate
        );
    }
}
