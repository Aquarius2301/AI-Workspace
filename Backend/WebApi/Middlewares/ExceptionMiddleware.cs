using System.Net;
using System.Text.Json;
using Infrastructure.Exceptions;

namespace WebApi.Middlewares
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
                _logger.LogError(ex, "Error: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Internal Server Error";

            if (exception is BaseException baseException)
            {
                statusCode = baseException.StatusCode;
                message = baseException.Message;
            }

            context.Response.StatusCode = (int)statusCode;

            var responseResult = JsonSerializer.Serialize(new { message });

            return context.Response.WriteAsync(responseResult);
        }
    }
}
