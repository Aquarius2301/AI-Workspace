using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Projects;

public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorCodes.ProjectNameRequired)
            .MaximumLength(100)
            .WithMessage(ErrorCodes.ProjectNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(ErrorCodes.TeamDescriptionMaxLength)
            .When(x => x.Description is not null);
    }
}
