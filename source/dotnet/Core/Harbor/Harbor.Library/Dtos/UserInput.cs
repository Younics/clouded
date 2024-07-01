using Newtonsoft.Json;

namespace Harbor.Library.Dtos;

public class UserInput
{
    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("username")]
    public string Name { get; set; } = null!;

    [JsonProperty("realname")]
    public string RealName { get; set; } = null!;

    [JsonProperty("password")]
    public string Password { get; set; } = null!;

    [JsonProperty("comment")]
    public string? Comment { get; set; }
}
