using Infrastructure.Exceptions;
using Infrastructure.Functions.Auth;
using Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly AuthSetting _authSetting;

    public AuthController(IMediator mediator, IOptions<AuthSetting> authSetting)
    {
        _mediator = mediator;
        _authSetting = authSetting.Value;
    }

    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("EmailPasswordRequired");

        var deviceInfo = HttpContext.Request.Headers.UserAgent.ToString();

        var result = await _mediator.Send(
            new LoginCommand(request.Email, request.Password, deviceInfo)
        );

        CookieHelper.AddCookie(
            HttpContext.Response,
            "access_token",
            result.AccessToken,
            TimeSpan.FromMinutes(_authSetting.AccessTokenMinutes).TotalDays
        );
        CookieHelper.AddCookie(
            HttpContext.Response,
            "refresh_token",
            result.RefreshToken,
            TimeSpan.FromDays(_authSetting.RefreshTokenDays).TotalDays
        );

        return Ok("Success");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new BadRequestException("Email is required");
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Password is required");

        await _mediator.Send(new RegisterCommand(request.Name, request.Email, request.Password));

        return Ok("Success");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = CookieHelper.GetCookie(HttpContext.Request, "refresh_token");

        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException();

        var deviceInfo = HttpContext.Request.Headers["User-Agent"].ToString();

        var result = await _mediator.Send(new RefreshCommand(refreshToken, deviceInfo));

        CookieHelper.AddCookie(
            HttpContext.Response,
            "access_token",
            result.AccessToken,
            TimeSpan.FromMinutes(_authSetting.AccessTokenMinutes).TotalDays
        );
        CookieHelper.AddCookie(
            HttpContext.Response,
            "refresh_token",
            result.RefreshToken,
            TimeSpan.FromDays(_authSetting.RefreshTokenDays).TotalDays
        );

        return Ok("Success");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = CookieHelper.GetCookie(HttpContext.Request, "refresh_token");

        if (!string.IsNullOrEmpty(refreshToken))
            await _mediator.Send(new LogoutCommand(refreshToken));

        CookieHelper.RemoveCookie(HttpContext.Response, "access_token");
        CookieHelper.RemoveCookie(HttpContext.Response, "refresh_token");

        return Ok("Success");
    }

    [Authorize]
    [HttpPost("revoke-all-refresh")]
    public async Task<IActionResult> RevokeAllRefresh()
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new RevokeAllRefreshCommand(userId));

        CookieHelper.RemoveCookie(HttpContext.Response, "access_token");
        CookieHelper.RemoveCookie(HttpContext.Response, "refresh_token");

        return Ok("Success");
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new MeQuery(userId));

        return Ok(result);
    }

    [Authorize]
    [HttpPost("me/active")]
    public async Task<IActionResult> UpdateActive()
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new UpdateActiveCommand(userId));

        return Ok("Success");
    }
}

public sealed record LoginRequest(string Email, string Password);

public sealed record RegisterRequest(string Name, string Email, string Password);
