namespace AIWorkspace.Application.Common.Models;

public sealed record PaginationRequest
{
    public PaginationRequest(
        int page,
        int pageSize,
        int maxPageSize = 100,
        int defaultPageSize = 20
    )
    {
        Page = page < 1 ? 1 : page;
        PageSize =
            pageSize < 1 ? defaultPageSize
            : pageSize > maxPageSize ? maxPageSize
            : pageSize;
    }

    public int Page { get; }
    public int PageSize { get; }
}

public sealed record PaginationResult<T>(int Page, int PageSize, int Total, List<T> Items);
