using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

public sealed record UpdateTaskCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    Guid TaskId,
    string? Title,
    string? Description,
    Guid? AssignedToId,
    TaskPriority? Priority,
    DateTimeOffset? DueDate,
    CancellationToken CancellationToken
) : IRequest<TaskItemResult>;

public sealed class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskItemResult>
{
    private static readonly string[] AllowedRoles = ["Admin", "CoAdmin", "ProjectLeader"];

    private readonly IAppDbContext _context;

    public UpdateTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItemResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context
            .TaskItems.Where(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId)
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TaskNotFound);

        var project = await _context
            .Projects.AsNoTracking()
            .Where(p => p.Id == request.ProjectId)
            .Select(p => new { p.TeamId, p.Name })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        var isTeamAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == request.CurrentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        var isProjectLeader = await _context
            .ProjectMembers.AsNoTracking()
            .AnyAsync(
                pm =>
                    pm.ProjectId == request.ProjectId
                    && pm.UserId == request.CurrentUserId
                    && pm.Role == ProjectRole.Leader,
                cancellationToken
            );

        if (!isTeamAdminOrCoAdmin && !isProjectLeader)
        {
            throw new ForbiddenException(ErrorCodes.Forbidden, new { AllowedRoles });
        }

        if (request.Title is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new BadRequestException(ErrorCodes.TaskTitleRequired);
            }

            task.Title = request.Title.Trim();
        }

        if (request.Description is not null)
        {
            task.Description = request.Description;
        }

        if (request.AssignedToId is not null)
        {
            task.AssignedToId = request.AssignedToId.Value;
        }

        if (request.Priority is not null)
        {
            task.Priority = request.Priority.Value;
        }

        if (request.DueDate is not null)
        {
            task.DueDate = request.DueDate.Value;
        }

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
