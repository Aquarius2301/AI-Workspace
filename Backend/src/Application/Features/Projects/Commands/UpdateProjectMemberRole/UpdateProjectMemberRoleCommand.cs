using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record UpdateProjectMemberRoleCommand(
    Guid UserId,
    Guid ProjectId,
    Guid MemberId,
    ProjectRole? Role,
    CancellationToken CancellationToken
) : IRequest;

public sealed class UpdateProjectMemberRoleCommandHandler
    : IRequestHandler<UpdateProjectMemberRoleCommand>
{
    private readonly IAppDbContext _context;

    public UpdateProjectMemberRoleCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        UpdateProjectMemberRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = request.UserId;
        var projectId = request.ProjectId;
        var memberId = request.MemberId;
        var newRole = request.Role;

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

        // Cannot change role of the project creator
        if (memberId == project.CreatorId)
            throw new BadRequestException(ErrorCodes.BadRequest);

        // Find the project member
        var projectMember = await _context.ProjectMembers.FirstOrDefaultAsync(
            pm => pm.ProjectId == projectId && pm.UserId == memberId,
            cancellationToken
        );

        if (projectMember is null)
            throw new NotFoundException(ErrorCodes.UserNotFound);

        // Update role if provided and different
        if (newRole.HasValue && newRole.Value != projectMember.Role)
        {
            projectMember.Role = newRole.Value;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
