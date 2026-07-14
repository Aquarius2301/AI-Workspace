using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;

namespace AIWorkspace.Application.Features.Projects;

public sealed record CreateProjectCommand(
    Guid CurrentUserId,
    Guid TeamId,
    string Name,
    string? Description,
    ProjectVisibility Visibility,
    CancellationToken CancellationToken
) : IRequest, IRequireTeamRole
{
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => CurrentUserId;
    TeamMemberRole[] IRequireTeamRole.AllowedRoles =>
        [TeamMemberRole.Admin, TeamMemberRole.CoAdmin];
}

public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand>
{
    private readonly IAppDbContext _context;

    public CreateProjectCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Slug = SlugHelper.GenerateSlug(),
            TeamId = request.TeamId,
            CreatorId = request.CurrentUserId,
            Name = request.Name,
            Description = request.Description,
            Visibility = request.Visibility,
        };

        _context.Projects.Add(project);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
