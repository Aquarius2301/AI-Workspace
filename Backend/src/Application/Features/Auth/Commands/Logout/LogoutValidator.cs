using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Auth;

public class LogoutValidator : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(ErrorCodes.RefreshTokenRequired);
    }
}
