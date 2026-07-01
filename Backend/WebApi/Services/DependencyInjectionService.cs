using DataAccess.Interceptors;
using DataAccess.Repositories;
using DataAccess.Services;
using DataAccess.UnitOfWork;
using Infrastructure.Behavior;
using Infrastructure.Functions.Auth;

namespace WebApi.Services;

public static class DependencyInjectionService
{
    public static IServiceCollection AddDIService(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register SQL capture service and interceptor
        services.AddScoped<ISqlCaptureService, SqlCaptureService>();
        services.AddScoped<SqlCaptureInterceptor>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
            cfg.AddOpenBehavior(typeof(TeamRoleBehavior<,>));
        });

        // Register HttpClient for ImageKitService
        services.AddHttpClient<ImageKitService>();

        return services;
    }
}
