using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Projects;

public sealed record GetIdProjectBySlugQuery(
    string Slug,
    Guid CurrentUserId,
    CancellationToken CancellationToken
) : IRequest<GetIdProjectResult>;

public sealed record GetIdProjectResult(Guid Id);

public sealed class GetIdProjectBySlugQueryHandler
    : IRequestHandler<GetIdProjectBySlugQuery, GetIdProjectResult>
{
    private readonly IAppDbContext _context;

    public GetIdProjectBySlugQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetIdProjectResult> Handle(
        GetIdProjectBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        var project =
            await _context
                .Projects.AsNoTracking()
                .Where(p => p.Slug == request.Slug)
                .Select(p => new { p.Id, p.TeamId })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.NotFound);

        // Check access: user must be a team Admin/CoAdmin or a project member
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
                pm => pm.ProjectId == project.Id && pm.UserId == request.CurrentUserId,
                cancellationToken
            );

        if (!isTeamAdminOrCoAdmin && !isProjectMember)
        {
            throw new NotFoundException(ErrorCodes.NotFound);
        }

        return new GetIdProjectResult(project.Id);
    }
}
