using BusinessObject;
using DataAccess.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services;

public static class DbService
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AIWorkspaceContext>(
            (serviceProvider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();

                // Add the SQL capture interceptor (scoped via service provider)
                var sqlCaptureInterceptor =
                    serviceProvider.GetRequiredService<SqlCaptureInterceptor>();
                options.AddInterceptors(sqlCaptureInterceptor);
            }
        );

        return services;
    }
}
