using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Commands;

public sealed record AddProjectMemberCommand(Guid CurrentUserId, Guid ProjectId, Guid UserId)
    : IRequest;

public sealed class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project =
            await _unitOfWork
                .Projects.GetQuery()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project not found");

        // Check permissions: Admin (any project) OR Leader (if creatorId matches)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole == TeamMemberRole.Admin)
        {
            // Admin can add members to any project
        }
        else if (teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId)
        {
            // Leader can only add members to projects they created
        }
        else
        {
            throw new ForbiddenException(
                "You do not have permission to add members to this project"
            );
        }

        // Verify the target user is a member of the team
        var isTeamMember = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm => tm.TeamId == project.TeamId && tm.UserId == request.UserId,
                cancellationToken
            );

        if (!isTeamMember)
            throw new BadRequestException("User is not a member of this team");

        // Check if already a project member
        var isAlreadyMember = await _unitOfWork
            .ProjectMembers.GetQuery()
            .AnyAsync(
                pm => pm.ProjectId == request.ProjectId && pm.UserId == request.UserId,
                cancellationToken
            );

        if (isAlreadyMember)
            throw new BadRequestException("User is already a member of this project");

        var projectMember = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            JoinedAt = DateTime.UtcNow,
        };

        _unitOfWork.ProjectMembers.Add(projectMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
