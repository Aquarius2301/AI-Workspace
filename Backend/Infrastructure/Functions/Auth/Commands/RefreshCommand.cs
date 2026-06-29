using System.Security.Cryptography;
using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Infrastructure.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Functions.Auth;

public sealed record RefreshCommand(
    string RefreshToken,
    string? DeviceInfo = null,
    CancellationToken CancellationToken = default
) : IRequest<RefreshResult>;

public sealed record RefreshResult(string AccessToken);

public sealed class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuthSetting _authSetting;

    public RefreshCommandHandler(IUnitOfWork unitOfWork, IOptions<AuthSetting> authSetting)
    {
        _unitOfWork = unitOfWork;
        _authSetting = authSetting.Value;
    }

    public async Task<RefreshResult> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        var storedToken = await _unitOfWork
            .RefreshTokens.GetQuery()
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken is null || storedToken.ExpiresAt < DateTimeOffset.UtcNow)
            throw new UnauthorizedException(ErrorCodes.InvalidRefreshToken);

        // Validate device info if present on both sides
        if (
            storedToken.DeviceInfo != null
            && request.DeviceInfo != null
            && storedToken.DeviceInfo != request.DeviceInfo
        )
            throw new UnauthorizedException(ErrorCodes.InvalidRefreshToken);

        var user =
            await _unitOfWork.Users.GetQuery().FirstOrDefaultAsync(u => u.Id == storedToken.UserId)
            ?? throw new UnauthorizedException(ErrorCodes.UserNotFound);

        // Generate new tokens
        var accessToken = JwtHelper.GenerateToken(user.Id, user.Email, _authSetting);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // storedToken.Token = refreshToken;

        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshResult(accessToken);
    }
}
