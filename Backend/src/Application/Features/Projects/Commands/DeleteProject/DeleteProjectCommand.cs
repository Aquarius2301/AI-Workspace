using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record DeleteProjectCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    CancellationToken CancellationToken
) : IRequest;

public sealed class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IAppDbContext _context;

    public DeleteProjectCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _context
                .Projects.Where(p => p.Id == request.ProjectId)
                .Select(p => new { p.Id, p.TeamId })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Authorize: only Team Admin/CoAdmin can delete a project
        var isTeamAdminOrCoAdmin = await _context
            .TeamMembers.AsNoTracking()
            .AnyAsync(
                tm =>
                    tm.TeamId == project.TeamId
                    && tm.UserId == request.CurrentUserId
                    && (tm.Role == TeamMemberRole.Admin || tm.Role == TeamMemberRole.CoAdmin),
                cancellationToken
            );

        if (!isTeamAdminOrCoAdmin)
        {
            throw new ForbiddenException(ErrorCodes.Forbidden);
        }

        // Delete tasks first (they depend on project)
        await _context
            .TaskItems.Where(t => t.ProjectId == request.ProjectId)
            .ExecuteDeleteAsync(cancellationToken);

        // Delete project members
        await _context
            .ProjectMembers.Where(pm => pm.ProjectId == request.ProjectId)
            .ExecuteDeleteAsync(cancellationToken);

        // Delete project
        await _context
            .Projects.Where(p => p.Id == request.ProjectId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}