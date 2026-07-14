using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Services;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record LoginCommand(
    string Email,
    string Password,
    string? DeviceInfo,
    string? DeviceId,
    CancellationToken CancellationToken
) : IRequest<LoginResult>;

public sealed record LoginResult(string AccessToken, string RefreshToken);

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAppDbContext _context;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IAppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user =
            await _context.Users.FirstOrDefaultAsync(
                x => x.Email == request.Email,
                cancellationToken
            ) ?? throw new UnauthorizedException(ErrorCodes.LoginFailed);

        if (!PasswordHasherHelper.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedException(ErrorCodes.LoginFailed);

        user.LastActiveAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: generate real JWT access token
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            DeviceInfo = request.DeviceInfo,
            DeviceId = request.DeviceId,
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, refreshToken);
    }
}
