using AIWorkspace.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record LogoutCommand(string RefreshToken, CancellationToken CancellationToken)
    : IRequest;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAppDbContext _context;

    public LogoutCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(
            x => x.Token == request.RefreshToken,
            cancellationToken
        );

        if (refreshToken is not null)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
