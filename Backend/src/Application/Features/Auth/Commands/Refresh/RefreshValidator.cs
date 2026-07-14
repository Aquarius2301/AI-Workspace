using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Auth;

public class RefreshValidator : AbstractValidator<RefreshCommand>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(ErrorCodes.RefreshTokenRequired);

        RuleFor(x => x.DeviceId).NotEmpty().WithMessage(ErrorCodes.DeviceIdRequired);
    }
}
