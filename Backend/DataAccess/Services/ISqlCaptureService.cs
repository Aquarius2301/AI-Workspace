namespace DataAccess.Services;

/// <summary>
/// Service to capture SQL queries executed during a request scope.
/// Used by RequestLoggingMiddleware to log executed queries.
/// </summary>
public interface ISqlCaptureService
{
    /// <summary>
    /// Gets the list of captured SQL queries.
    /// </summary>
    IReadOnlyList<string> CapturedQueries { get; }

    /// <summary>
    /// Captures a SQL query.
    /// </summary>
    void Capture(string sql);

    /// <summary>
    /// Clears all captured queries.
    /// </summary>
    void Clear();
}
