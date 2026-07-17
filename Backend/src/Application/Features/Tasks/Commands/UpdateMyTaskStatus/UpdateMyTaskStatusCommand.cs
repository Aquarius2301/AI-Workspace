using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

public sealed record UpdateMyTaskStatusCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    Guid TaskId,
    TaskItemStatus NewStatus,
    CancellationToken CancellationToken
) : IRequest<TaskItemResult>;

public sealed class UpdateMyTaskStatusCommandHandler
    : IRequestHandler<UpdateMyTaskStatusCommand, TaskItemResult>
{
    private readonly IAppDbContext _context;

    public UpdateMyTaskStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItemResult> Handle(
        UpdateMyTaskStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        var task =
            await _context
                .TaskItems.Where(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId)
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TaskNotFound);

        // Verify the user has access to the project
        var project = await _context
            .Projects.AsNoTracking()
            .Where(p => p.Id == request.ProjectId)
            .Select(p => new { p.TeamId })
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(ErrorCodes.ProjectNotFound);
        }

        var isTeamAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == request.CurrentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        var isProjectMember = await _context
            .ProjectMembers.AsNoTracking()
            .AnyAsync(
                pm => pm.ProjectId == request.ProjectId && pm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamAdminOrCoAdmin && !isProjectMember)
        {
            throw new NotFoundException(ErrorCodes.NotFound);
        }

        // Only the assigned user can update their own task status
        if (task.AssignedToId != request.CurrentUserId)
        {
            throw new ForbiddenException(ErrorCodes.Forbidden);
        }

        task.Status = request.NewStatus;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToResult(task);
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
            task.Priority,
            task.Status,
            task.CreatedAt,
            task.DueDate
        );
    }
}
