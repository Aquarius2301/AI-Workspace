using System.Text;
using System.Text.Json;
using DataAccess.Services;

namespace WebApi.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only log API requests (controllers), skip static files, swagger, etc.
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        // Enable buffering so we can read the request body multiple times
        context.Request.EnableBuffering();

        // Read request body
        string requestBody = await ReadRequestBodyAsync(context.Request);

        // Capture the response body
        var originalResponseBody = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        // Resolve SQL capture service from request scope
        var sqlCaptureService = context.RequestServices.GetService<ISqlCaptureService>();
        // Clear any previously captured queries from the scope
        sqlCaptureService?.Clear();

        try
        {
            await _next(context);

            // Read response body
            string responseBody = await ReadResponseBodyAsync(context.Response);

            // Log to file
            await WriteLogAsync(context, requestBody, responseBody, sqlCaptureService);
        }
        finally
        {
            // Copy the buffered response back to the original stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.Body == null || request.ContentLength == null || request.ContentLength == 0)
            return string.Empty;

        try
        {
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true
            );
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin); // Reset position for downstream
            return body;
        }
        catch
        {
            return "<unable to read>";
        }
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        try
        {
            using var reader = new StreamReader(
                response.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true
            );
            var body = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin); // Reset position for writing back
            return body;
        }
        catch
        {
            return "<unable to read>";
        }
    }

    private async Task WriteLogAsync(
        HttpContext context,
        string requestBody,
        string responseBody,
        ISqlCaptureService? sqlCaptureService
    )
    {
        try
        {
            var now = DateTime.Now;
            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(logDir);

            var logFile = Path.Combine(logDir, $"api-{now:yyyy-MM-dd}.log");

            var logEntry = new StringBuilder();
            logEntry.AppendLine($"--- [{now:yyyy-MM-dd HH:mm:ss}] ---");
            logEntry.AppendLine(
                $"Request: {context.Request.Method} {context.Request.Path}{context.Request.QueryString}"
            );
            logEntry.AppendLine($"Body: {FormatJson(requestBody)}");

            // Log captured SQL queries if any
            var capturedQueries = sqlCaptureService?.CapturedQueries;
            if (capturedQueries != null && capturedQueries.Count > 0)
            {
                logEntry.AppendLine($"SQL ({capturedQueries.Count} queries):");
                foreach (var sql in capturedQueries)
                {
                    logEntry.AppendLine(sql);
                    logEntry.AppendLine("---");
                }
            }

            logEntry.AppendLine($"Response: {FormatJson(responseBody)}");
            logEntry.AppendLine(new string('-', 80));

            await File.AppendAllTextAsync(logFile, logEntry.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write API log file");
        }
    }

    private static string FormatJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        try
        {
            using var doc = JsonDocument.Parse(text);
            return JsonSerializer.Serialize(
                doc.RootElement,
                new JsonSerializerOptions { WriteIndented = false }
            );
        }
        catch
        {
            return text;
        }
    }
}
