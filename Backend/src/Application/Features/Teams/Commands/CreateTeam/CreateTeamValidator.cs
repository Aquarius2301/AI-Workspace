using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Teams;

public class CreateTeamValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorCodes.TeamNameRequired)
            .MaximumLength(100)
            .WithMessage(ErrorCodes.TeamNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(ErrorCodes.TeamDescriptionMaxLength)
            .When(x => x.Description is not null);
    }
}
