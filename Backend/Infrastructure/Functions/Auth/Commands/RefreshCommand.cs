using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Infrastructure.Functions.Auth;

public sealed record RefreshCommand(string RefreshToken, string? DeviceInfo = null)
    : IRequest<RefreshResult>;

public sealed record RefreshResult(string AccessToken, string RefreshToken);

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
        var storedToken = _unitOfWork
            .RefreshTokens.GetQuery()
            .FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (storedToken is null || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid or expired refresh token");

        // Validate device info if present on both sides
        if (
            storedToken.DeviceInfo != null
            && request.DeviceInfo != null
            && storedToken.DeviceInfo != request.DeviceInfo
        )
            throw new UnauthorizedException("Invalid or expired refresh token");

        var user =
            _unitOfWork.Users.GetQuery().FirstOrDefault(u => u.Id == storedToken.UserId)
            ?? throw new UnauthorizedException("User not found");

        // Generate new tokens
        var accessToken = JwtHelper.GenerateToken(user.Id, user.Email, _authSetting);
        var refreshToken = Guid.NewGuid().ToString();

        // Tạo refresh token record mới
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_authSetting.RefreshTokenDays),
            DeviceInfo = request.DeviceInfo,
        };

        // Thêm record mới vào DB
        _unitOfWork.RefreshTokens.Add(newRefreshToken);

        // Xóa record cũ sau khi tạo mới thành công
        _unitOfWork.RefreshTokens.Remove(storedToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshResult(accessToken, refreshToken);
    }
}
