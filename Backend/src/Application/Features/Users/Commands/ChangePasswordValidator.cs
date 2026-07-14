using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Users;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage(ErrorCodes.PasswordRequired);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(ErrorCodes.PasswordRequired)
            .MinimumLength(8)
            .WithMessage(ErrorCodes.PasswordInvalid)
            .Must(password => password.Any(char.IsUpper))
            .WithMessage(ErrorCodes.PasswordInvalid)
            .Must(password => password.Any(ch => !char.IsLetterOrDigit(ch)))
            .WithMessage(ErrorCodes.PasswordInvalid);
    }
}
