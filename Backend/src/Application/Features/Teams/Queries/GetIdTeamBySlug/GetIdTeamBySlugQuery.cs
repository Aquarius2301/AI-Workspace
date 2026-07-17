using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetIdTeamBySlugQuery(
    string Slug,
    Guid CurrentUserId,
    CancellationToken CancellationToken
) : IRequest<GetTeamResult> { }

public sealed record GetIdTeamResult(Guid Id);

public sealed class GetIdTeamBySlugQueryHandler
    : IRequestHandler<GetIdTeamBySlugQuery, GetTeamResult>
{
    private readonly IAppDbContext _context;

    public GetIdTeamBySlugQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetTeamResult> Handle(
        GetIdTeamBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        var team =
            await _context
                .Teams.AsNoTracking()
                .Where(t =>
                    t.Slug == request.Slug
                    && t.TeamMembers.Any(tm => tm.UserId == request.CurrentUserId)
                )
                .Select(t => new GetTeamResult(
                    t.Id,
                    t.Name,
                    t.Description,
                    t.TeamMembers.Where(tm => tm.UserId == request.CurrentUserId)
                        .Select(tm => tm.Role)
                        .First()
                ))
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.TeamNotFound);

        return team;
    }
}
