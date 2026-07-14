using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Common.Behaviors;

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
    private readonly IAppDbContext _context;

    public TeamRoleBehavior(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var teamMember =
            await _context
                .TeamMembers.AsNoTracking()
                .Where(tm => tm.TeamId == request.TeamId && tm.UserId == request.CurrentUserId)
                .Select(tm => new { tm.Role })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ForbiddenException(
                ErrorCodes.Forbidden,
                new { AllowedRoles = request.AllowedRoles.Select(r => r.ToString()).ToArray() }
            );

        if (!request.AllowedRoles.Contains(teamMember.Role))
        {
            throw new ForbiddenException(
                ErrorCodes.Forbidden,
                new { AllowedRoles = request.AllowedRoles.Select(r => r.ToString()).ToArray() }
            );
        }

        return await next();
    }
}
