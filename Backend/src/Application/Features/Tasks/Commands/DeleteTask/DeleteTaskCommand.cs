using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Tasks;

public sealed record DeleteTaskCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    Guid TaskId,
    CancellationToken CancellationToken
) : IRequest;

public sealed class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private static readonly string[] AllowedRoles = ["Admin", "CoAdmin", "ProjectLeader"];

    private readonly IAppDbContext _context;

    public DeleteTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context
            .TaskItems.Where(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TaskNotFound);

        var project = await _context
            .Projects.AsNoTracking()
            .Where(p => p.Id == request.ProjectId)
            .Select(p => new { p.TeamId })
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

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
