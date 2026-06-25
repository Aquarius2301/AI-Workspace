namespace WebApi.Services;

public static class WebService
{
    public static IServiceCollection AddWebCors(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var frontendUrl = configuration["Frontend:Url"] ?? "http://localhost:5173";

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigin",
                policy =>
                {
                    policy
                        .WithOrigins(frontendUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    ;
                }
            );
        });

        return services;
    }
}
