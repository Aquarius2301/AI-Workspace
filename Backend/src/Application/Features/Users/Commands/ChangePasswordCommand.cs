using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Users;

public sealed record ChangePasswordCommand(
    Guid CurrentUserId,
    string OldPassword,
    string NewPassword,
    CancellationToken CancellationToken
) : IRequest;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IAppDbContext _context;

    public ChangePasswordCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user =
            await _context.Users.FirstOrDefaultAsync(
                x => x.Id == request.CurrentUserId,
                cancellationToken
            ) ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        if (!PasswordHasherHelper.Verify(user.PasswordHash, request.OldPassword))
        {
            throw new UnauthorizedException(ErrorCodes.PasswordInvalid);
        }

        user.PasswordHash = PasswordHasherHelper.Hash(request.NewPassword);
        user.LastActiveAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
