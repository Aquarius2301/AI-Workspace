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

    /// <summary>
    /// Log in with email and password, returns access and refresh tokens in cookies
    /// </summary>
    /// <param name="request">An object containing email and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// On successful authentication, two HTTP-only cookies are set:
    /// - <c>access_token</c>: Short-lived JWT for API authorization
    /// - <c>refresh_token</c>: Long-lived token for obtaining new access tokens
    /// This endpoint is rate-limited by the "LoginPolicy" to prevent brute force attacks.
    /// </remarks>
    /// <response code="200">Success if login is successful</response>
    /// <response code="400">EmailPasswordRequired if email or password is missing</response>
    /// <response code="401">InvalidEmailOrPassword if email or password is incorrect</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException(ErrorCodes.EmailPasswordRequired);

        var deviceInfo = HttpContext.Request.Headers.UserAgent.ToString();

        var result = await _mediator.Send(
            new LoginCommand(request.Email, request.Password, deviceInfo, cancellationToken)
        );

        CookieHelper.AddCookie(
            HttpContext.Response,
            "access_token",
            result.AccessToken,
            TimeSpan.FromMinutes(_authSetting.AccessTokenMinutes)
        );
        CookieHelper.AddCookie(
            HttpContext.Response,
            "refresh_token",
            result.RefreshToken,
            TimeSpan.FromDays(_authSetting.RefreshTokenDays)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Register a new user account with name, email and password
    /// </summary>
    /// <param name="request">An object containing name, email and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Password requirements:
    /// - Minimum 8 characters
    /// - At least one uppercase letter
    /// - At least one special character
    /// </remarks>
    /// <response code="200">Success if registration is successful</response>
    /// <response code="400">
    /// - NameRequired if name is missing
    /// - EmailRequired if email is missing
    /// - PasswordRequired if password is missing
    /// - PasswordInvalid if password does not meet requirements
    /// </response>
    /// <response code="409">EmailAlreadyExists if the email is already taken</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException(ErrorCodes.NameRequired);
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new BadRequestException(ErrorCodes.EmailRequired);
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException(ErrorCodes.PasswordRequired);
        await _mediator.Send(
            new RegisterCommand(request.Name, request.Email, request.Password, cancellationToken)
        );

        return Ok("Success");
    }

    /// <summary>
    /// Refresh an access token using a refresh token stored in cookies
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Reads the <c>refresh_token</c> cookie from the request.
    /// If valid, new access and refresh tokens are generated and set as HTTP-only cookies.
    /// </remarks>
    /// <response code="200">Success if token refresh is successful; new tokens are set in cookies</response>
    /// <response code="401">
    /// - Unauthorized if refresh token cookie is missing
    /// - InvalidRefreshToken if refresh token is expired or invalid
    /// - UserNotFound if the associated user no longer exists
    /// </response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = CookieHelper.GetCookie(HttpContext.Request, "refresh_token");

        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException(ErrorCodes.InvalidRefreshToken);

        var deviceInfo = HttpContext.Request.Headers.UserAgent.ToString();

        var result = await _mediator.Send(
            new RefreshCommand(refreshToken, deviceInfo, cancellationToken)
        );

        CookieHelper.AddCookie(
            HttpContext.Response,
            "access_token",
            result.AccessToken,
            TimeSpan.FromMinutes(_authSetting.AccessTokenMinutes)
        );

        // CookieHelper.AddCookie(
        //     HttpContext.Response,
        //     "refresh_token",
        //     result.RefreshToken,
        //     TimeSpan.FromDays(_authSetting.RefreshTokenDays)
        // );

        return Ok("Success");
    }

    /// <summary>
    /// Log out by removing the refresh token from the database and clearing cookies
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Removes the <c>refresh_token</c> from the database if it exists,
    /// then clears both <c>access_token</c> and <c>refresh_token</c> cookies.
    /// </remarks>
    /// <response code="200">Success if logout is processed</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = CookieHelper.GetCookie(HttpContext.Request, "refresh_token");

        if (!string.IsNullOrEmpty(refreshToken))
            await _mediator.Send(new LogoutCommand(refreshToken, cancellationToken));

        CookieHelper.RemoveCookie(HttpContext.Response, "access_token");
        CookieHelper.RemoveCookie(HttpContext.Response, "refresh_token");

        return Ok("Success");
    }

    /// <summary>
    /// Revoke all refresh tokens for the current user and clear cookies
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Requires authentication.
    /// All existing refresh tokens for the user are deleted.
    /// Both <c>access_token</c> and <c>refresh_token</c> cookies are cleared.
    /// </remarks>
    /// <response code="200">Success if all refresh tokens are revoked</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [Authorize]
    [HttpPost("revoke-all-refresh")]
    public async Task<IActionResult> RevokeAllRefresh(CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new RevokeAllRefreshCommand(userId, cancellationToken));

        CookieHelper.RemoveCookie(HttpContext.Response, "access_token");
        CookieHelper.RemoveCookie(HttpContext.Response, "refresh_token");

        return Ok("Success");
    }

    /// <summary>
    /// Get the current authenticated user's profile information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns the user's avatar, name, email and language settings</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="404">UserNotFound if the authenticated user is not found</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var result = await _mediator.Send(new MeQuery(userId, cancellationToken));

        return Ok(result);
    }

    /// <summary>
    /// Update the current user's last active timestamp
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Requires authentication.
    /// Updates the <c>LastActiveAt</c> field of the current user to the current UTC time.
    /// </remarks>
    /// <response code="200">Success if active timestamp is updated</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    /// <response code="404">UserNotFound if the authenticated user is not found</response>
    /// <response code="500">InternalServerError if an internal server error occurs</response>
    [Authorize]
    [HttpPost("me/active")]
    public async Task<IActionResult> UpdateActive(CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        await _mediator.Send(new UpdateActiveCommand(userId, cancellationToken));

        return Ok("Success");
    }
}

public sealed record LoginRequest(string Email, string Password);

public sealed record RegisterRequest(string Name, string Email, string Password);
