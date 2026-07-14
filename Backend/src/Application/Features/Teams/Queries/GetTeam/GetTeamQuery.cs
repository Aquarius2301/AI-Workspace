using System.Runtime.CompilerServices;
using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Teams;

public sealed record GetTeamQuery(Guid Id, Guid CurrentUserId, CancellationToken CancellationToken)
    : IRequest<GetTeamResult> { };

public sealed record GetTeamResult(Guid Id, string Name, string? Description, TeamMemberRole Role);

public sealed class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, GetTeamResult>
{
    private readonly IAppDbContext _context;

    public GetTeamQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetTeamResult> Handle(
        GetTeamQuery request,
        CancellationToken cancellationToken
    )
    {
        var team =
            await _context
                .Teams.AsNoTracking()
                .Where(t =>
                    t.Id == request.Id
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
