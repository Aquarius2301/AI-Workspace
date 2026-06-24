using System.Security.Cryptography;
using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Infrastructure.Functions.Auth;

public sealed record LoginCommand(string Email, string Password, string? DeviceInfo = null)
    : IRequest<LoginResult>;

public sealed record LoginResult(string AccessToken, string RefreshToken);

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuthSetting _authSetting;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IOptions<AuthSetting> authSetting)
    {
        _unitOfWork = unitOfWork;
        _authSetting = authSetting.Value;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = _unitOfWork.Users.GetQuery().FirstOrDefault(u => u.Email == request.Email);

        if (user is null)
            throw new UnauthorizedException(ErrorCodes.InvalidEmailOrPassword);

        if (!PasswordHelper.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedException(ErrorCodes.InvalidEmailOrPassword);

        var accessToken = JwtHelper.GenerateToken(user.Id, user.Email, _authSetting);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_authSetting.RefreshTokenDays),
            DeviceInfo = request.DeviceInfo,
        };

        _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, refreshTokenValue);
    }
}
