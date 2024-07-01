using Harbor.Library.Enums;
using Newtonsoft.Json;

namespace Harbor.Library.Dtos;

public class ProjectMemberAddInput
{
    [JsonProperty("member_user")]
    public MemberInput Member { get; set; } = null!;

    [JsonProperty("role_id")]
    public ERole Role { get; set; }
}
