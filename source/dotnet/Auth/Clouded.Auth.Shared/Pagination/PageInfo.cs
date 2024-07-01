namespace Clouded.Auth.Shared.Pagination;

public class PageInfo
{
    public long PageIndex { get; init; }
    public long PageSize { get; init; }
    public long TotalElements { get; init; }
    public long TotalPages { get; init; }
}
