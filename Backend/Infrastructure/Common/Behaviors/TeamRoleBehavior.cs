using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Behavior;

/// <summary>
/// Interface for requests that require role-based authorization within a team.
/// Implement this on MediatR command/query records to enable automatic role checking
/// via the <see cref="TeamRoleBehavior{TRequest,TResponse}"/> pipeline behavior.
/// </summary>
public interface IRequireTeamRole
{
    Guid TeamId { get; }
    Guid CurrentUserId { get; }
    TeamMemberRole[] AllowedRoles { get; }
}

/// <summary>
/// MediatR pipeline behavior that automatically enforces role requirements
/// for any request implementing <see cref="IRequireTeamRole"/>.
/// </summary>
public sealed class TeamRoleBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequireTeamRole
{
    private readonly IUnitOfWork _unitOfWork;

    public TeamRoleBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var hasRole = await _unitOfWork
            .TeamMembers.GetQuery()
            .AnyAsync(
                tm =>
                    tm.TeamId == request.TeamId
                    && tm.UserId == request.CurrentUserId
                    && request.AllowedRoles.Contains(tm.Role),
                cancellationToken
            );

        if (!hasRole)
        {
            var allowedRoleNames = string.Join(
                ", ",
                request.AllowedRoles.Select(r => r.ToString())
            );
            throw new ForbiddenException($"Require one of roles: {allowedRoleNames}");
        }

        return await next();
    }
}
