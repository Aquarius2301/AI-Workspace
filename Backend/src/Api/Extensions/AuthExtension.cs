using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AIWorkspace.Application.Common;
using AIWorkspace.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AIWorkspace.Api.Extensions;

public static class AuthServiceExtension
{
    public static IServiceCollection AddJwtAuth(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var authSection = configuration.GetSection("AuthSetting");
        services.Configure<AuthSetting>(authSection);

        var key = configuration.GetValue<string>("AuthSetting:JwtKey");
        if (!string.IsNullOrEmpty(key))
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetValue<string>("AuthSetting:Issuer"),
                        ValidAudience = configuration.GetValue<string>("AuthSetting:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue("access_token", out var token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            var userIdClaim = context.Principal?.FindFirstValue(
                                JwtRegisteredClaimNames.Sub
                            );
                            var deviceId = context.Principal?.FindFirstValue("deviceId");

                            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(deviceId))
                            {
                                context.Fail("Missing required claims (userId or deviceId)");
                                return;
                            }

                            var dbContext =
                                context.HttpContext.RequestServices.GetRequiredService<IAppDbContext>();

                            var hasValidSession = await dbContext.RefreshTokens.AnyAsync(t =>
                                t.UserId == Guid.Parse(userIdClaim)
                                && t.DeviceId == deviceId
                                && t.ExpiresAt > DateTimeOffset.UtcNow
                            );

                            if (!hasValidSession)
                            {
                                context.Fail("Session has been revoked");
                            }
                        },
                    };
                });

            services.AddAuthorization();
        }

        return services;
    }
}
