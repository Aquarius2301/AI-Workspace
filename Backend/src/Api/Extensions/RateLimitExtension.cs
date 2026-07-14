using System.Text.Json;
using System.Threading.RateLimiting;
using AIWorkspace.Api.Filters;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Infrastructure.Settings;
using Microsoft.AspNetCore.RateLimiting;

namespace AIWorkspace.Api.Extensions;

public static class RateLimitExtension
{
    public static IServiceCollection AddRateLimit(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var rateLimitSetting =
            configuration.GetSection(RateLimitSetting.SectionName).Get<RateLimitSetting>()
            ?? new RateLimitSetting();

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(
                "LoginPolicy",
                opt =>
                {
                    opt.PermitLimit = rateLimitSetting.PermitLimit;
                    opt.Window = TimeSpan.FromMinutes(rateLimitSetting.WindowMinutes);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                }
            );

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var response = ApiResponse.Fail(ErrorCodes.TooManyRequests);
                var json = JsonSerializer.Serialize(
                    response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                await context.HttpContext.Response.WriteAsync(json, cancellationToken);
            };
        });

        return services;
    }
}
