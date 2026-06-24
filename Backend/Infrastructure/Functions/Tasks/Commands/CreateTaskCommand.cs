using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Tasks.Commands;

public sealed record CreateTaskCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssignedToId,
    int Priority,
    DateTime? DueDate
) : IRequest;

public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project =
            await _unitOfWork
                .Projects.GetQuery()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project not found");

        // Check permissions: Admin (any project) OR Leader (if creatorId matches)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole == TeamMemberRole.Admin)
        {
            // Admin can create tasks in any project in their team
        }
        else if (teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId)
        {
            // Leader can only create tasks in projects they created
        }
        else
        {
            throw new ForbiddenException(
                "You do not have permission to create tasks in this project"
            );
        }

        // If assignedToId is provided, verify user is a member of the project's team
        if (request.AssignedToId.HasValue && request.AssignedToId.Value != Guid.Empty)
        {
            var isMember = await _unitOfWork
                .TeamMembers.GetQuery()
                .AnyAsync(
                    tm => tm.TeamId == project.TeamId && tm.UserId == request.AssignedToId.Value,
                    cancellationToken
                );

            if (!isMember)
                throw new BadRequestException("Assigned user is not a member of this team");
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            Title = request.Title,
            Description = request.Description,
            AssignedToId = request.AssignedToId,
            Priority = request.Priority,
            Status = TaskItemStatus.Open,
            CreatedAt = DateTime.UtcNow,
            DueDate = request.DueDate,
        };

        _unitOfWork.TaskItems.Add(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
