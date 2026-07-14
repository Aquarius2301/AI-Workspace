using AIWorkspace.Infrastructure;
using AIWorkspace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Api.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            // Only log sensitive data in development to avoid leaking PII in production logs
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}
