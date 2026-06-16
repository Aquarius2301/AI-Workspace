using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Commands;

public sealed record UpdateTaskCommand(
    Guid CurrentUserId,
    Guid TaskId,
    string? Title,
    string? Description,
    DateTime? DueDate
) : IRequest;

public sealed class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
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
            // Admin can update any task in their team
        }
        else if (
            teamRole == TeamMemberRole.Leader
            && task.Project.CreatorId == request.CurrentUserId
        )
        {
            // Leader can only update tasks in projects they created
        }
        else
        {
            throw new ForbiddenException("You do not have permission to update this task");
        }

        if (request.Title is not null)
            task.Title = request.Title;

        if (request.Description is not null)
            task.Description = request.Description;

        if (request.DueDate.HasValue)
            task.DueDate = request.DueDate;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
