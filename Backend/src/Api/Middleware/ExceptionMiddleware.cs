using System.Net;
using System.Text.Json;
using AIWorkspace.Api.Filters;
using AIWorkspace.Application.Common.Exceptions;

namespace AIWorkspace.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Only log as Error for 5xx server errors; 4xx client errors should be Warning level
                if (ex is BaseException baseEx && (int)baseEx.StatusCode < 500)
                {
                    _logger.LogWarning(
                        ex,
                        "Client error ({StatusCode}): {Message}",
                        (int)baseEx.StatusCode,
                        ex.Message
                    );
                }
                else
                {
                    _logger.LogError(ex, "Server error: {Message}", ex.Message);
                }
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = ErrorCodes.InternalServerError;
            object? data = null;

            if (exception is BaseException baseException)
            {
                statusCode = baseException.StatusCode;
                message = baseException.Message;
                data = baseException.Data;
            }

            context.Response.StatusCode = (int)statusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var responseResult = JsonSerializer.Serialize(
                ApiResponse.Fail(message, data),
                jsonOptions
            );

            return context.Response.WriteAsync(responseResult);
        }
    }
}
