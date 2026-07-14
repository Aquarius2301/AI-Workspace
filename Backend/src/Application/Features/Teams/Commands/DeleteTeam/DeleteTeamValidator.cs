using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Teams;

public class DeleteTeamValidator : AbstractValidator<DeleteTeamCommand>
{
    public DeleteTeamValidator()
    {
        // No validation rules needed - authorization handled by TeamRoleBehavior
    }
}
