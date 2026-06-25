using System.Collections.Concurrent;

namespace DataAccess.Services;

/// <summary>
/// Scoped service that captures SQL queries executed during a request.
/// </summary>
public class SqlCaptureService : ISqlCaptureService
{
    private readonly List<string> _queries = new();

    public IReadOnlyList<string> CapturedQueries => _queries.AsReadOnly();

    public void Capture(string sql)
    {
        _queries.Add(sql);
    }

    public void Clear()
    {
        _queries.Clear();
    }
}
