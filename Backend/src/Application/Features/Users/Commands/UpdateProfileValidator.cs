using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Users;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorCodes.NameRequired)
            .MaximumLength(100)
            .WithMessage(ErrorCodes.NameMaxLength)
            .When(x => x.Name is not null);
    }
}
