using Infrastructure.Settings;

public static class SettingService
{
    public static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AuthSetting>(configuration.GetSection("AuthSettings"));

        return services;
    }
}
