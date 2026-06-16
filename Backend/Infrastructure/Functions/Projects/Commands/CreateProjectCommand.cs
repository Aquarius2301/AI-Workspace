using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using MediatR;

namespace Infrastructure.Functions.Projects.Commands;

public sealed record CreateProjectCommand(
    Guid CurrentUserId,
    Guid TeamId,
    string Name,
    string? Description,
    ProjectVisibility Visibility
) : IRequest<ProjectResponse>, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin, TeamMemberRole.Leader];
    Guid IRequireTeamRole.TeamId => TeamId;
    Guid IRequireTeamRole.CurrentUserId => CurrentUserId;
}

public sealed record ProjectResponse(Guid Id, string Name, string? Description, string Visibility);

public sealed class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, ProjectResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectResponse> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken
    )
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TeamId = request.TeamId,
            CreatorId = request.CurrentUserId,
            Name = request.Name,
            Description = request.Description,
            Visibility = request.Visibility,
        };

        _unitOfWork.Projects.Add(project);

        // Add creator to project members if it's a private project
        if (request.Visibility == ProjectVisibility.Private)
        {
            var projectMember = new ProjectMember
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                UserId = request.CurrentUserId,
                JoinedAt = DateTime.UtcNow,
            };
            _unitOfWork.ProjectMembers.Add(projectMember);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.Visibility.ToString()
        );
    }
}
