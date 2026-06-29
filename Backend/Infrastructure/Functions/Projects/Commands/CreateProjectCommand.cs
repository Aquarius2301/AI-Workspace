using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects;

public sealed record CreateProjectCommand(
    Guid CurrentUserId,
    Guid TeamId,
    string Name,
    string? Description,
    ProjectVisibility Visibility,
    CancellationToken CancellationToken = default
) : IRequest, IRequireTeamRole
{
    TeamMemberRole[] IRequireTeamRole.AllowedRoles => [TeamMemberRole.Admin, TeamMemberRole.Leader];
    Guid IRequireTeamRole.TeamId => TeamId;
}

public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Validate name is not empty
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException(ErrorCodes.NameRequired);

        // Check team exists
        var team = await _unitOfWork
            .Teams.GetQuery()
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken);

        if (team is null)
            throw new NotFoundException(ErrorCodes.TeamNotFound);

        // Create project
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TeamId = request.TeamId,
            CreatorId = request.CurrentUserId,
            Name = request.Name,
            Description = request.Description,
            Visibility = request.Visibility,
        };

        // Create project member (creator is automatically a member)
        var projectMember = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            UserId = request.CurrentUserId,
            JoinedAt = DateTimeOffset.UtcNow,
        };

        _unitOfWork.Projects.Add(project);
        _unitOfWork.ProjectMembers.Add(projectMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
