namespace WebApi.Common;

/// <summary>
/// Standard API response wrapper for endpoints that do not return data.
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse Ok(string message = "Success") =>
        new() { Success = true, Message = message };

    public static ApiResponse Fail(string message) => new() { Success = false, Message = message };
}

/// <summary>
/// Standard API response wrapper for endpoints that return data.
/// </summary>
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data,
        };

    public static new ApiResponse<T> Fail(string message) =>
        new()
        {
            Success = false,
            Message = message,
            Data = default,
        };
}
