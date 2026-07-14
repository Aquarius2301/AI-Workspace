using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Auth;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorCodes.EmailRequired);

        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorCodes.PasswordRequired);
    }
}
