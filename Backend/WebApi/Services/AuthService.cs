using System.Text;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Services;

public static class AuthService
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
                                // Console.WriteLine($"[JWT Auth] Token found in cookies: {token[..Math.Min(15, token.Length)]}...");
                            }
                            // else
                            // {
                            // Console.WriteLine("[JWT Auth] Token NOT found in cookies.");
                            // }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            // Console.WriteLine($"[JWT Auth] Authentication failed: {context.Exception.Message}");
                            if (context.Exception.InnerException != null)
                            {
                                // Console.WriteLine($"[JWT Auth] Inner Exception: {context.Exception.InnerException.Message}");
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            // Console.WriteLine($"[JWT Auth] Token validated successfully for user: {context.Principal?.Identity?.Name ?? "Unknown"}");
                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddAuthorization();
        }

        return services;
    }
}
