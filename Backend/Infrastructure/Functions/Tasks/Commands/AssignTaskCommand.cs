using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Commands;

public sealed record AssignTaskCommand(Guid CurrentUserId, Guid TaskId, Guid AssignedToId)
    : IRequest;

public sealed class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var task =
            await _unitOfWork
                .TaskItems.GetQuery()
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TaskNotFound);

        // Check permissions: Admin (any project) OR Leader (if creatorId matches)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole == TeamMemberRole.Admin)
        {
            // Admin can assign any task in their team
        }
        else if (
            teamRole == TeamMemberRole.Leader
            && task.Project.CreatorId == request.CurrentUserId
        )
        {
            // Leader can only assign tasks in projects they created
        }
        else
        {
            throw new ForbiddenException(ErrorCodes.NoPermissionAssignTask);
        }

        // Verify the assignee is a member of the project's team
        var isMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.AssignedToId,
                cancellationToken
            );

        if (!isMember)
            throw new BadRequestException(ErrorCodes.AssignedUserNotTeamMember);

        task.AssignedToId = request.AssignedToId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
