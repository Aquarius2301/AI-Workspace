namespace AIWorkspace.Api.Filters;

/// <summary>
/// Standard API response wrapper for endpoints that do not return data.
/// </summary>
public class ApiResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    public static ApiResponse Ok(string message = "Success") =>
        new() { Status = true, Message = message };

    public static ApiResponse Fail(string message, object? data = null) =>
        new()
        {
            Status = false,
            Message = message,
            Data = data,
        };
}

/// <summary>
/// Standard API response wrapper for endpoints that return data.
/// </summary>
public class ApiResponse<T> : ApiResponse
{
    public new T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new()
        {
            Status = true,
            Message = message,
            Data = data,
        };

    public static new ApiResponse<T> Fail(string message, object? data = null) =>
        new()
        {
            Status = false,
            Message = message,
            Data = default,
        };
}
