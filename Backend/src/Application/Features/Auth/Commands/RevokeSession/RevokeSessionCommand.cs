using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record RevokeSessionCommand(Guid UserId, string DeviceId) : IRequest;

public sealed class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand>
{
    private readonly IAppDbContext _context;

    public RevokeSessionCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
    {
        var refreshToken =
            await _context.RefreshTokens.FirstOrDefaultAsync(
                x => x.UserId == request.UserId && x.DeviceId == request.DeviceId,
                cancellationToken
            ) ?? throw new NotFoundException();

        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
