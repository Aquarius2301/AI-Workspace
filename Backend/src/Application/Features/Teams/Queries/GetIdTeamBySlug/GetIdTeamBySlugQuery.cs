using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetIdTeamBySlugQuery(
    string Slug,
    Guid CurrentUserId,
    CancellationToken CancellationToken
) : IRequest<GetIdTeamResult> { }

public sealed record GetIdTeamResult(Guid Id);

public sealed class GetIdTeamBySlugQueryHandler
    : IRequestHandler<GetIdTeamBySlugQuery, GetIdTeamResult>
{
    private readonly IAppDbContext _context;

    public GetIdTeamBySlugQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetIdTeamResult> Handle(
        GetIdTeamBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        var team =
            await _context
                .Teams.AsNoTracking()
                .FirstOrDefaultAsync(
                    t =>
                        t.Slug == request.Slug
                        && t.TeamMembers.Any(tm => tm.UserId == request.CurrentUserId),
                    cancellationToken
                )
            ?? throw new NotFoundException(ErrorCodes.TeamNotFound);

        return new GetIdTeamResult(team.Id);
    }
}
