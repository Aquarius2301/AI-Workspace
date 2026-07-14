using System.Text.Json;
using AIWorkspace.Api.Filters;

namespace AIWorkspace.Api.Extensions;

public static class ControllerExtension
{
    public static IServiceCollection AddController(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Filters.Add<ApiResponseFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter(
                        JsonNamingPolicy.CamelCase
                    )
                );
            });
        return services;
    }
}
