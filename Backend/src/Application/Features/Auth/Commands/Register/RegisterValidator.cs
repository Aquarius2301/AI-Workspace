using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Auth;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorCodes.NameRequired);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ErrorCodes.EmailRequired)
            .EmailAddress()
            .WithMessage(ErrorCodes.EmailInvalid);

        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorCodes.PasswordRequired);

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage(ErrorCodes.PasswordInvalid)
            .Must(password => password.Any(char.IsUpper))
            .WithMessage(ErrorCodes.PasswordInvalid)
            .Must(password => password.Any(ch => !char.IsLetterOrDigit(ch)))
            .WithMessage(ErrorCodes.PasswordInvalid);
    }
}
