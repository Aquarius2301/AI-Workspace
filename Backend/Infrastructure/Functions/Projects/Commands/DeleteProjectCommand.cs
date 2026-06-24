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
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.ProjectNotFound);

        // Check permissions: Admin in team OR (Leader/Admin of project creator)
        var teamRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .Where(tm => tm.TeamId == project.TeamId && tm.UserId == request.CurrentUserId)
            .Select(tm => tm.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamRole != TeamMemberRole.Admin && teamRole != TeamMemberRole.Leader)
            throw new ForbiddenException(ErrorCodes.NoPermissionDeleteProject);

        if (teamRole == TeamMemberRole.Leader && project.CreatorId != request.CurrentUserId)
            throw new ForbiddenException(ErrorCodes.NoPermissionDeleteProject);

        if (project.ProjectMembers.Count > 0)
            _unitOfWork.ProjectMembers.RemoveRange(project.ProjectMembers);

        _unitOfWork.Projects.Remove(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
