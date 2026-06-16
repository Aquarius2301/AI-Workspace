using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Infrastructure.Functions.Auth;

public sealed record RefreshCommand(string RefreshToken) : IRequest<RefreshResult>;

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

        var user = _unitOfWork.Users.GetQuery().FirstOrDefault(u => u.Id == storedToken.UserId);

        if (user is null)
            throw new UnauthorizedException("User not found");

        // Generate new tokens
        var accessToken = JwtHelper.GenerateToken(user.Id, user.Email, _authSetting);
        var refreshToken = Guid.NewGuid().ToString();

        // Save the new refresh token
        storedToken.Token = refreshToken;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshResult(accessToken, refreshToken);
    }
}
