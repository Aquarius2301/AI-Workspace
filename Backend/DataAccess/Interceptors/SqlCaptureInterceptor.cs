using System.Data.Common;
using System.Text;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataAccess.Interceptors;

/// <summary>
/// EF Core interceptor that captures executed SQL queries and stores them
/// in the scoped <see cref="ISqlCaptureService"/> for logging purposes.
/// This interceptor should be registered as a scoped service in DI.
/// </summary>
public class SqlCaptureInterceptor : IDbCommandInterceptor
{
    private readonly ISqlCaptureService _sqlCaptureService;

    public SqlCaptureInterceptor(ISqlCaptureService sqlCaptureService)
    {
        _sqlCaptureService = sqlCaptureService;
    }

    public ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default
    )
    {
        CaptureSql(command);
        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    public ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default
    )
    {
        CaptureSql(command);
        return new ValueTask<InterceptionResult<object>>(result);
    }

    public ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        CaptureSql(command);
        return new ValueTask<InterceptionResult<int>>(result);
    }

    private void CaptureSql(DbCommand command)
    {
        _sqlCaptureService.Capture(FormatSql(command));
    }

    private static string FormatSql(DbCommand command)
    {
        var sb = new StringBuilder();
        sb.AppendLine(command.CommandText.TrimEnd());

        if (command.Parameters.Count > 0)
        {
            sb.AppendLine("/* Parameters:");
            foreach (DbParameter parameter in command.Parameters)
            {
                var value = parameter.Value == DBNull.Value ? "NULL" : parameter.Value;
                sb.AppendLine($"   {parameter.ParameterName} = {value}");
            }
            sb.Append("*/");
        }

        return sb.ToString().TrimEnd();
    }
}
