using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record UpdateProjectCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    string? Name,
    string? Description,
    ProjectVisibility? Visibility,
    CancellationToken CancellationToken
) : IRequest;

public sealed class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private static readonly string[] AllowedRoles = ["Admin", "CoAdmin", "ProjectLeader"];

    private readonly IAppDbContext _context;

    public UpdateProjectCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project =
            await _context.Projects.FirstOrDefaultAsync(
                p => p.Id == request.ProjectId,
                cancellationToken
            ) ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Allow if user is a Team Admin/CoAdmin or a Project Leader
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

        if (request.Name is not null)
            project.Name = request.Name;

        if (request.Description is not null)
            project.Description = request.Description;

        if (request.Visibility is not null)
            project.Visibility = request.Visibility.Value;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
