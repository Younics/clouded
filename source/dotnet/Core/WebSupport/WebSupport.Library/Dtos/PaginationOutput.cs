using Newtonsoft.Json;

namespace WebSupport.Library.Dtos;

public class PaginationOutput<T>
    where T : class, new()
{
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();

    [JsonProperty("pager")]
    public PaginationInfo PageInfo { get; set; } = new();
}

public class PaginationInfo
{
    [JsonProperty("page")]
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    [JsonProperty("items")]
    public int TotalSize { get; set; }
}
