using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

public sealed record CreateTaskCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssignedToId,
    TaskPriority Priority,
    DateTimeOffset? DueDate,
    CancellationToken CancellationToken
) : IRequest<TaskItemResult>;

public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskItemResult>
{
    private static readonly string[] AllowedRoles = ["Admin", "CoAdmin", "ProjectLeader"];

    private readonly IAppDbContext _context;

    public CreateTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItemResult> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _context
                .Projects.AsNoTracking()
                .Where(p => p.Id == request.ProjectId)
                .Select(p => new { p.TeamId, p.Name })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Authorize: user must be Team Admin/CoAdmin or Project Leader
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

        // Resolve assigned user name if provided
        string? assignedToName = null;
        if (request.AssignedToId.HasValue)
        {
            assignedToName = await _context
                .Users.AsNoTracking()
                .Where(u => u.Id == request.AssignedToId.Value)
                .Select(u => u.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            Title = request.Title,
            Description = request.Description,
            AssignedToId = request.AssignedToId,
            Priority = request.Priority,
            Status = TaskItemStatus.ToDo,
            CreatedAt = DateTimeOffset.UtcNow,
            DueDate = request.DueDate,
        };

        _context.TaskItems.Add(task);

        await _context.SaveChangesAsync(cancellationToken);

        return new TaskItemResult(
            task.Id,
            task.ProjectId,
            project.Name,
            task.Title,
            task.Description,
            task.AssignedToId,
            assignedToName,
            task.Priority,
            task.Status,
            task.CreatedAt,
            task.DueDate
        );
    }
}
