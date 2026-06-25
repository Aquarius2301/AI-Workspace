using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApi.Services;

public static class RateLimitService
{
    public static IServiceCollection AddRateLimit(this IServiceCollection services)
    {
        // Rate limiting: tối đa 5 request login/phút từ 1 IP
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(
                "LoginPolicy",
                opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                }
            );

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }
}
