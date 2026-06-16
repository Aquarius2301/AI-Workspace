using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Commands;

public sealed record UpdateTaskStatusCommand(Guid CurrentUserId, Guid TaskId, TaskItemStatus Status)
    : IRequest;

public sealed class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task =
            await _unitOfWork
                .TaskItems.GetQuery()
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task not found");

        // Check permissions:
        // Admin (any project) OR Leader (if they created the project) OR Assignee (the user assigned to this task)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        var isAdmin = teamRole == TeamMemberRole.Admin;
        var isLeaderOfProject =
            teamRole == TeamMemberRole.Leader && task.Project.CreatorId == request.CurrentUserId;
        var isAssignee = task.AssignedToId == request.CurrentUserId;

        if (!isAdmin && !isLeaderOfProject && !isAssignee)
        {
            throw new ForbiddenException(
                "You do not have permission to change the status of this task"
            );
        }

        task.Status = request.Status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
