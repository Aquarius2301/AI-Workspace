using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Teams;

public class UpdateTeamValidator : AbstractValidator<UpdateTeamCommand>
{
    public UpdateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name is not null)
            .WithMessage(ErrorCodes.TeamNameRequired)
            .MaximumLength(100)
            .When(x => x.Name is not null)
            .WithMessage(ErrorCodes.TeamNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(ErrorCodes.TeamDescriptionMaxLength)
            .When(x => x.Description is not null);
    }
}
