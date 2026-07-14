using System.Text;
using System.Text.Json;

namespace AIWorkspace.Api.Middlewares;

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

        try
        {
            await _next(context);

            // Read response body
            string responseBody = await ReadResponseBodyAsync(context.Response);

            // Log to file
            await WriteLogAsync(context, requestBody, responseBody);
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

            // Mask sensitive fields before logging
            return MaskSensitiveFields(body);
        }
        catch
        {
            return "<unable to read>";
        }
    }

    /// <summary>
    /// Masks sensitive fields (password, token, secret) in request body before logging.
    /// </summary>
    private static string MaskSensitiveFields(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        try
        {
            using var doc = JsonDocument.Parse(json);
            var sensitiveKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "password",
                "newPassword",
                "oldPassword",
                "secret",
                "token",
                "access_token",
                "refresh_token",
                "apiKey",
                "privateKey",
            };

            var root = new Dictionary<string, JsonElement?>();
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                root[property.Name] = sensitiveKeys.Contains(property.Name)
                    ? null // Will be replaced with "***"
                    : property.Value;
            }

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(
                stream,
                new JsonWriterOptions { SkipValidation = true }
            );
            writer.WriteStartObject();
            foreach (var (key, value) in root)
            {
                if (sensitiveKeys.Contains(key))
                {
                    writer.WriteString(key, "***");
                }
                else if (value.HasValue)
                {
                    value.Value.WriteTo(writer);
                }
            }
            writer.WriteEndObject();
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
        catch
        {
            // If parsing fails (e.g., not JSON), return original but try basic pattern masking
            return MaskSensitivePatterns(json);
        }
    }

    private static string MaskSensitivePatterns(string text)
    {
        // Basic pattern-based masking for non-JSON bodies (e.g., form data)
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var sensitivePatterns = new[] { "password=", "token=", "secret=" };
        var result = text;
        foreach (var pattern in sensitivePatterns)
        {
            int idx;
            while ((idx = result.IndexOf(pattern, StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                var endIdx = result.IndexOf('&', idx);
                if (endIdx < 0)
                    endIdx = result.Length;
                result = result[..(idx + pattern.Length)] + "***" + result[endIdx..];
            }
        }
        return result;
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

    private async Task WriteLogAsync(HttpContext context, string requestBody, string responseBody)
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
