using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Behaviors;
using AIWorkspace.Application.Common.Services;
using AIWorkspace.Application.Features.Auth;
using AIWorkspace.Infrastructure.Persistence;
using AIWorkspace.Infrastructure.Services;
using FluentValidation;

namespace AIWorkspace.Api.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddDIService(this IServiceCollection services)
    {
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(LoginValidator).Assembly);

        services.AddScoped<IJwtService, JwtService>();

        services.AddHttpClient<IImageKitService, ImageKitService>();

        return services;
    }
}
