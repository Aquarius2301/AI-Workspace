using AIWorkspace.Infrastructure.Settings;

namespace AIWorkspace.Api.Extensions;

public static class SettingExtension
{
    public static IServiceCollection AddSetting(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AuthSetting>(configuration.GetSection("AuthSetting"));
        services.Configure<UploadSetting>(configuration.GetSection("UploadSetting"));
        services.Configure<RateLimitSetting>(configuration.GetSection("RateLimitSetting"));
        services.Configure<FrontendSetting>(configuration.GetSection("FrontendSetting"));

        return services;
    }
}
