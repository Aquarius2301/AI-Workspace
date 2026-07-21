using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Projects;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name is not null)
            .WithMessage(ErrorCodes.ProjectNameRequired)
            .MaximumLength(100)
            .When(x => x.Name is not null)
            .WithMessage(ErrorCodes.ProjectNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(ErrorCodes.TeamDescriptionMaxLength)
            .When(x => x.Description is not null);
    }
}
