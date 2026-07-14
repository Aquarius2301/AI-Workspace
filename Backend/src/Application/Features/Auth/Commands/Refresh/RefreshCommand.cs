using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Services;
using AIWorkspace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record RefreshCommand(
    string RefreshToken,
    string DeviceId,
    int RefreshTokenDays,
    CancellationToken CancellationToken
) : IRequest<RefreshResult>;

public sealed record RefreshResult(string AccessToken, string RefreshToken);

public sealed class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshResult>
{
    private readonly IAppDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshCommandHandler(IAppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<RefreshResult> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        var oldRefreshToken =
            await _context.RefreshTokens.FirstOrDefaultAsync(
                x =>
                    x.Token == request.RefreshToken
                    && x.DeviceId == request.DeviceId
                    && x.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken
            ) ?? throw new UnauthorizedException();

        var user =
            await _context
                .Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == oldRefreshToken.UserId, cancellationToken)
            ?? throw new UnauthorizedException();

        // SECURITY: Invalidate old refresh token (prevent refresh token reuse attack)
        _context.RefreshTokens.Remove(oldRefreshToken);

        // Generate new access token
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, request.DeviceId);

        // Generate new refresh token with ABSOLUTE expiry (based on original CreatedAt)
        // This ensures the user must re-login after RefreshTokenDays regardless of how many times they refresh
        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();
        var expiresAt = oldRefreshToken.CreatedAt.AddDays(request.RefreshTokenDays);
        if (expiresAt <= DateTimeOffset.UtcNow)
        {
            // Absolute window has expired, force re-login
            throw new UnauthorizedException();
        }

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshTokenValue,
            CreatedAt = oldRefreshToken.CreatedAt, // Keep original creation time for absolute expiry
            ExpiresAt = expiresAt,
            DeviceInfo = oldRefreshToken.DeviceInfo,
            DeviceId = request.DeviceId,
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshResult(accessToken, newRefreshTokenValue);
    }
}
