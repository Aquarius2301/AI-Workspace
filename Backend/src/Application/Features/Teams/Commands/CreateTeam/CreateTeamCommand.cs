using AIWorkspace.Application.Common;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;

namespace AIWorkspace.Application.Features.Teams;

public sealed record CreateTeamCommand(
    Guid UserId,
    string Name,
    string? Description,
    CancellationToken CancellationToken
) : IRequest<CreateTeamResult>;

public sealed record CreateTeamResult(string Slug);

public sealed class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, CreateTeamResult>
{
    private readonly IAppDbContext _context;

    public CreateTeamCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateTeamResult> Handle(
        CreateTeamCommand request,
        CancellationToken cancellationToken
    )
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Slug = SlugHelper.GenerateSlug(),
            Name = request.Name,
            Description = request.Description,
        };

        var teamMember = new TeamMember
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            UserId = request.UserId,
            Role = TeamMemberRole.Admin,
            JoinedAt = DateTimeOffset.UtcNow,
        };

        _context.Teams.Add(team);
        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateTeamResult(team.Slug);
    }
}
