using Newtonsoft.Json;

namespace Harbor.Library.Dtos;

public class ProjectInput
{
    [JsonProperty("project_name")]
    public string Name { get; set; } = null!;

    [JsonProperty("public")]
    public bool IsPublic { get; set; }

    [JsonProperty("storage_limit")]
    public int StorageLimit { get; set; }
}
