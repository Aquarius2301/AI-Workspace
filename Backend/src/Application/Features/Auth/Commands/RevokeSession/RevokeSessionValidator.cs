using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Auth;

public class RevokeSessionValidator : AbstractValidator<RevokeSessionCommand>
{
    public RevokeSessionValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty().WithMessage(ErrorCodes.DeviceIdRequired);
    }
}
