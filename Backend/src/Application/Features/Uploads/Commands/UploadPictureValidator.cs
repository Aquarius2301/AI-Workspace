using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;

namespace AIWorkspace.Application.Features.Uploads;

public class UploadPictureValidator : AbstractValidator<UploadPictureCommand>
{
    public UploadPictureValidator()
    {
        RuleFor(x => x.FileStream).NotNull().WithMessage(ErrorCodes.FileRequired);

        RuleFor(x => x.FileName).NotEmpty().WithMessage(ErrorCodes.FileRequired);
    }
}
