using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Users;

public sealed record UpdateProfileCommand(
    Guid CurrentUserId,
    string? Name,
    string? FileId,
    LanguageDisplay? Language,
    CancellationToken CancellationToken
) : IRequest;

public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IAppDbContext _context;

    public UpdateProfileCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user =
            await _context.Users.FirstOrDefaultAsync(
                x => x.Id == request.CurrentUserId,
                cancellationToken
            ) ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        if (request.Name is not null)
        {
            user.Name = request.Name;
        }

        if (request.FileId is not null)
        {
            // Get old picture
            var oldPicture = await _context.Pictures.FirstOrDefaultAsync(
                p => p.UsedByUserId == request.CurrentUserId,
                cancellationToken
            );

            if (oldPicture is not null)
            {
                oldPicture.IsActive = false;
                oldPicture.UsedByUserId = null;
            }

            var picture =
                await _context.Pictures.FirstOrDefaultAsync(
                    p => p.FileId == request.FileId,
                    cancellationToken
                ) ?? throw new NotFoundException(ErrorCodes.PictureNotFound);

            picture.UsedByUserId = request.CurrentUserId;

            user.AvatarPictureId = picture.Id;
        }

        if (request.Language.HasValue)
        {
            user.Language = request.Language.Value;
        }

        user.LastActiveAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
