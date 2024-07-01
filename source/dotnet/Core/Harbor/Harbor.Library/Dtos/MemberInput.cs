using Newtonsoft.Json;

namespace Harbor.Library.Dtos;

public class MemberInput
{
    [JsonProperty("username")]
    public string? UserName { get; set; }

    [JsonProperty("user_id")]
    public long UserId { get; set; }
}
