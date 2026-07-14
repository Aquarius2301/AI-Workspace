using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Auth;

public class RevokeAllRefreshValidator : AbstractValidator<RevokeAllRefreshCommand>
{
    public RevokeAllRefreshValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage(ErrorCodes.UserIdRequired);
    }
}
