namespace AIWorkspace.Api.Extensions;

public static class AllExtension
{
    public static IServiceCollection AddExtensions(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        services.AddJwtAuth(configuration);
        services.AddCors(configuration);
        services.AddController();
        services.AddDIService();
        services.AddDatabase(configuration, environment);
        services.AddRateLimit(configuration);
        services.AddSetting(configuration);
        services.AddSwagger();

        return services;
    }
}
