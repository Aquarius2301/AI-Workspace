using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Features.Auth;
using AIWorkspace.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace AIWorkspace.Api.Controllers;

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

    /// <summary>
    /// Logs in a user using their email and password.
    /// </summary>
    /// <remarks>
    /// If the request does not contain a DeviceId cookie, one will be generated automatically
    /// and stored in a cookie (valid for 10 years).
    /// On successful login, an AccessToken and RefreshToken are issued and stored as HttpOnly cookies.
    /// </remarks>
    /// <param name="request">Login credentials containing email and password.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Login successful.</response>
    /// <response code="400">Email or Password is missing (EmailRequired, PasswordRequired).</response>
    /// <response code="404">Invalid email or password (LoginFailed).</response>
    /// <response code="429">Too many login attempts. Rate limit exceeded.</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var deviceInfo = HttpContext.Request.Headers.UserAgent.ToString();

        var deviceId = CookieHelper.GetCookie(HttpContext.Request, CookieKey.DeviceId);

        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = Guid.NewGuid().ToString();
            CookieHelper.AddCookie(
                HttpContext.Response,
                CookieKey.DeviceId,
                deviceId,
                TimeSpan.FromDays(3650)
            );
        }

        var result = await _mediator.Send(
            new LoginCommand(
                request.Email,
                request.Password,
                deviceInfo,
                deviceId,
                cancellationToken
            )
        );

        CookieHelper.AddCookie(
            HttpContext.Response,
            CookieKey.AccessToken,
            result.AccessToken,
            TimeSpan.FromMinutes(_authSetting.AccessTokenMinutes)
        );
        CookieHelper.AddCookie(
            HttpContext.Response,
            CookieKey.RefreshToken,
            result.RefreshToken,
            TimeSpan.FromDays(_authSetting.RefreshTokenDays)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token stored in a cookie.
    /// </summary>
    /// <remarks>
    /// The refresh token and device ID are read from HttpOnly cookies (refresh_token and device_id).
    /// If the refresh token is valid and not expired, a new access token is issued and stored as an HttpOnly cookie.
    /// </remarks>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Access token refreshed successfully.</response>
    /// <response code="400">Refresh token or Device ID is missing from cookies (RefreshTokenRequired, DeviceIdRequired).</response>
    /// <response code="401">Refresh token is invalid, expired, or device ID does not match (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = CookieHelper.GetCookie(HttpContext.Request, CookieKey.RefreshToken);

        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException();

        var deviceId =
            CookieHelper.GetCookie(HttpContext.Request, CookieKey.DeviceId)
            ?? throw new UnauthorizedException();

        var result = await _mediator.Send(
            new RefreshCommand(
                refreshToken,
                deviceId,
                _authSetting.RefreshTokenDays,
                cancellationToken
            )
        );

        CookieHelper.AddCookie(
            HttpContext.Response,
            CookieKey.AccessToken,
            result.AccessToken,
            TimeSpan.FromMinutes(_authSetting.AccessTokenMinutes)
        );
        CookieHelper.AddCookie(
            HttpContext.Response,
            CookieKey.RefreshToken,
            result.RefreshToken,
            TimeSpan.FromDays(_authSetting.RefreshTokenDays)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Logs out the current user by revoking the refresh token and clearing authentication cookies.
    /// </summary>
    /// <remarks>
    /// The refresh token is read from the refresh_token cookie and revoked (removed from the database).
    /// Both the access_token and refresh_token cookies are cleared from the response.
    /// If no refresh token cookie exists, the method simply clears the cookies without throwing an error.
    /// </remarks>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Logout successful.</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = CookieHelper.GetCookie(HttpContext.Request, CookieKey.RefreshToken);

        if (!string.IsNullOrEmpty(refreshToken))
            await _mediator.Send(new LogoutCommand(refreshToken, cancellationToken));

        CookieHelper.RemoveCookie(HttpContext.Response, CookieKey.AccessToken);
        CookieHelper.RemoveCookie(HttpContext.Response, CookieKey.RefreshToken);

        return Ok("Success");
    }

    /// <summary>
    /// Revokes all active sessions (refresh tokens) for the current user, logging them out from every device.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// All refresh tokens belonging to the user are removed from the database,
    /// and the access_token and refresh_token cookies are cleared.
    /// </remarks>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">All sessions revoked successfully.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [Authorize]
    [HttpDelete("sessions/all")]
    public async Task<IActionResult> RevokeAllRefresh(CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new RevokeAllRefreshCommand(userId, cancellationToken));

        CookieHelper.RemoveCookie(HttpContext.Response, CookieKey.AccessToken);
        CookieHelper.RemoveCookie(HttpContext.Response, CookieKey.RefreshToken);

        return Ok("Success");
    }

    /// <summary>
    /// Retrieves all active sessions (logged-in devices) for the current user.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// The current device ID is read from the device_id cookie to determine which session is the current one.
    /// Returns a list of sessions with device information, creation/expiration dates, and an indicator of the current session.
    /// </remarks>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns a list of active sessions (SessionResult[]).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var currentDeviceId = CookieHelper.GetCookie(HttpContext.Request, CookieKey.DeviceId);

        var result = await _mediator.Send(
            new GetSessionsQuery(userId, currentDeviceId),
            cancellationToken
        );

        return Ok(result);
    }

    /// <summary>
    /// Revokes a specific session (device) for the current user.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// The specified device's refresh token is removed from the database, effectively logging out that device.
    /// </remarks>
    /// <param name="deviceId">The ID of the device/session to revoke.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Session revoked successfully.</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="404">No session found for the specified device ID (NotFound).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [Authorize]
    [HttpDelete("sessions/{deviceId}")]
    public async Task<IActionResult> RevokeSession(
        string deviceId,
        CancellationToken cancellationToken
    )
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new RevokeSessionCommand(userId, deviceId), cancellationToken);

        return Ok("Success");
    }

    /// <summary>
    /// Retrieves the profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// Returns user profile information including ID, avatar URL, name, email, and language preference.
    /// </remarks>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the current user's profile (MeResult).</response>
    /// <response code="401">User is not authenticated or the account no longer exists (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new MeQuery(userId, cancellationToken));

        return Ok(result);
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// This endpoint does not require authentication. It creates a new user with the provided name, email, and password.
    /// The password is hashed using PBKDF2 before storage.
    /// After successful registration, the user must log in to receive tokens.
    /// </remarks>
    /// <param name="request">Registration details including name, email, and password.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Validation failed: email already exists, or name/email/password format is invalid (EmailAlreadyExists, NameRequired, NameMaxLength, EmailInvalid, PasswordInvalid).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        await _mediator.Send(
            new RegisterCommand(request.Name, request.Email, request.Password, cancellationToken)
        );

        return Ok("Success");
    }
}

public sealed record LoginRequest(string Email, string Password);

public sealed record RegisterRequest(string Name, string Email, string Password);
