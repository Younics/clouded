namespace Clouded.Auth.Shared.Pagination;

public class Paginated<T>
    where T : class
{
    /// <summary>
    /// Paginated data items
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Result page info
    /// </summary>
    public PageInfo PageInfo { get; set; } = new();
}
