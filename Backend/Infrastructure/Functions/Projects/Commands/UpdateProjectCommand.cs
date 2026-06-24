using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Commands;

public sealed record UpdateProjectCommand(
    Guid CurrentUserId,
    Guid ProjectId,
    string? Name,
    string? Description,
    ProjectVisibility? Visibility
) : IRequest;

public sealed class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project =
            await _unitOfWork
                .Projects.GetQuery()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Check permissions: Admin (any project) OR Leader (if creatorId matches)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole == TeamMemberRole.Admin)
        {
            // Admin can update any project in their team
        }
        else if (teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId)
        {
            // Leader can only update projects they created
        }
        else
        {
            throw new ForbiddenException(ErrorCodes.NoPermissionUpdateProject);
        }

        if (request.Name is not null)
            project.Name = request.Name;

        if (request.Description is not null)
            project.Description = request.Description;

        if (request.Visibility.HasValue)
        {
            var oldVisibility = project.Visibility;
            project.Visibility = request.Visibility.Value;

            // If changing from Private to Public, remove all project members
            if (
                oldVisibility == ProjectVisibility.Private
                && request.Visibility.Value == ProjectVisibility.Public
            )
            {
                var projectMembers = await _unitOfWork
                    .ProjectMembers.GetQuery()
                    .Where(pm => pm.ProjectId == request.ProjectId)
                    .ToListAsync(cancellationToken);

                _unitOfWork.ProjectMembers.RemoveRange(projectMembers);
            }

            // If changing from Public to Private, add the creator as project member
            if (
                oldVisibility == ProjectVisibility.Public
                && request.Visibility.Value == ProjectVisibility.Private
            )
            {
                var existingMember = await _unitOfWork
                    .ProjectMembers.GetQuery()
                    .AnyAsync(
                        pm =>
                            pm.ProjectId == request.ProjectId && pm.UserId == request.CurrentUserId,
                        cancellationToken
                    );

                if (!existingMember)
                {
                    _unitOfWork.ProjectMembers.Add(
                        new BusinessObject.Entities.ProjectMember
                        {
                            Id = Guid.NewGuid(),
                            ProjectId = request.ProjectId,
                            UserId = request.CurrentUserId,
                            JoinedAt = DateTime.UtcNow,
                        }
                    );
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
