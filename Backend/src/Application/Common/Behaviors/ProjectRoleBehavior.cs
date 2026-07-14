using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Common.Behaviors;

/// <summary>
/// Interface for requests that require role-based authorization within a team.
/// Implement this on MediatR command/query records to enable automatic role checking
/// via the <see cref="ProjectRoleBehavior{TRequest,TResponse}"/> pipeline behavior.
/// </summary>
public interface IRequireProjectRole
{
    Guid ProjectId { get; }
    Guid CurrentUserId { get; }
    ProjectRole[] AllowedRoles { get; }
}

/// <summary>
/// MediatR pipeline behavior that automatically enforces role requirements
/// for any request implementing <see cref="IRequireProjectRole"/>.
/// </summary>
public sealed class ProjectRoleBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequireProjectRole
{
    private readonly IAppDbContext _context;

    public ProjectRoleBehavior(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var projectMember =
            await _context
                .ProjectMembers.AsNoTracking()
                .Where(tm =>
                    tm.ProjectId == request.ProjectId && tm.UserId == request.CurrentUserId
                )
                .Select(tm => new { tm.Role })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ForbiddenException(
                ErrorCodes.Forbidden,
                new { AllowedRoles = request.AllowedRoles.Select(r => r.ToString()).ToArray() }
            );

        if (!request.AllowedRoles.Contains(projectMember.Role))
        {
            throw new ForbiddenException(
                ErrorCodes.Forbidden,
                new { AllowedRoles = request.AllowedRoles.Select(r => r.ToString()).ToArray() }
            );
        }

        return await next();
    }
}
