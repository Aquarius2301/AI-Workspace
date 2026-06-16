using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Projects.Commands;

public sealed record DeleteProjectCommand(Guid CurrentUserId, Guid ProjectId) : IRequest;

public sealed class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
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
            // Admin can delete any project in their team
        }
        else if (teamRole == TeamMemberRole.Leader && project.CreatorId == request.CurrentUserId)
        {
            // Leader can only delete projects they created
        }
        else
        {
            throw new ForbiddenException("You do not have permission to delete this project");
        }

        // Remove all project members first
        var projectMembers = await _unitOfWork
            .ProjectMembers.GetQuery()
            .Where(pm => pm.ProjectId == request.ProjectId)
            .ToListAsync(cancellationToken);

        _unitOfWork.ProjectMembers.RemoveRange(projectMembers);

        _unitOfWork.Projects.Remove(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
