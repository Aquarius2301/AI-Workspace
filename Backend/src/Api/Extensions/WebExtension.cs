using AIWorkspace.Infrastructure.Settings;

namespace AIWorkspace.Api.Extensions;

public static class CORSExtension
{
    public static IServiceCollection AddCors(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var frontendSetting =
            configuration.GetSection(FrontendSetting.SectionName).Get<FrontendSetting>()
            ?? new FrontendSetting();

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigin",
                policy =>
                {
                    policy
                        .WithOrigins(frontendSetting.Url)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

        return services;
    }
}
