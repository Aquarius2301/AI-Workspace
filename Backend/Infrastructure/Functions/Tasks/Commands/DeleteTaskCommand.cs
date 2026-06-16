using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Commands;

public sealed record DeleteTaskCommand(Guid CurrentUserId, Guid TaskId) : IRequest;

public sealed class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task =
            await _unitOfWork
                .TaskItems.GetQuery()
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task not found");

        // Check permissions: Admin (any project) OR Leader (if creatorId matches)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == task.Project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole == TeamMemberRole.Admin)
        {
            // Admin can delete any task in their team
        }
        else if (
            teamRole == TeamMemberRole.Leader
            && task.Project.CreatorId == request.CurrentUserId
        )
        {
            // Leader can only delete tasks in projects they created
        }
        else
        {
            throw new ForbiddenException("You do not have permission to delete this task");
        }

        // Remove related comments and attachments
        var comments = await _unitOfWork
            .Comments.GetQuery()
            .Where(c =>
                c.ReferenceType == ReferenceType.TaskItem && c.ReferenceId == request.TaskId
            )
            .ToListAsync(cancellationToken);

        var attachments = await _unitOfWork
            .Attachments.GetQuery()
            .Where(a =>
                a.ReferenceType == ReferenceType.TaskItem && a.ReferenceId == request.TaskId
            )
            .ToListAsync(cancellationToken);

        _unitOfWork.Comments.RemoveRange(comments);
        _unitOfWork.Attachments.RemoveRange(attachments);
        _unitOfWork.TaskItems.Remove(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
