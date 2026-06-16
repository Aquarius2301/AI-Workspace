namespace WebApi.Services;

public static class AllService
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        AuthService.AddJwtAuth(services, configuration);
        DependencyInjectionService.AddDIService(services);
        DbService.AddDatabase(services, configuration);
        SettingService.AddSettings(services, configuration);
        SwaggerService.AddSwagger(services);
        WebService.AddWebCors(services, configuration);
        return services;
    }
}
