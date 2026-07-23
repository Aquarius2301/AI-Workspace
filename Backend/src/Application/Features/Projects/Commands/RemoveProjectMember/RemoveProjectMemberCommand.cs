using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record RemoveProjectMemberCommand(
    Guid UserId,
    Guid ProjectId,
    Guid MemberId,
    CancellationToken CancellationToken
) : IRequest;

public sealed class RemoveProjectMemberCommandHandler : IRequestHandler<RemoveProjectMemberCommand>
{
    private readonly IAppDbContext _context;

    public RemoveProjectMemberCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        RemoveProjectMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = request.UserId;
        var projectId = request.ProjectId;
        var memberId = request.MemberId;

        // Get project info
        var project = await _context
            .Projects.AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => new { p.TeamId, p.CreatorId })
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
            throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Authorization: current user must be Admin or CoAdmin of the team
        var isAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == currentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        if (!isAdminOrCoAdmin)
            throw new ForbiddenException(ErrorCodes.Forbidden);

        // Cannot remove the project creator
        if (memberId == project.CreatorId)
            throw new BadRequestException(ErrorCodes.BadRequest);

        // Find the project member
        var projectMember = await _context.ProjectMembers.FirstOrDefaultAsync(
            pm => pm.ProjectId == projectId && pm.UserId == memberId,
            cancellationToken
        );

        if (projectMember is null)
            throw new NotFoundException(ErrorCodes.UserNotFound);

        // Unassign all tasks assigned to this member in this project
        var assignedTasks = await _context
            .TaskItems.Where(t => t.ProjectId == projectId && t.AssignedToId == memberId)
            .ToListAsync(cancellationToken);

        foreach (var task in assignedTasks)
        {
            task.AssignedToId = null;
        }

        // Remove the member
        _context.ProjectMembers.Remove(projectMember);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
