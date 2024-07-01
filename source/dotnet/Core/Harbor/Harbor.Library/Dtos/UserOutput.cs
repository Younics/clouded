using Newtonsoft.Json;

namespace Harbor.Library.Dtos;

public class UserOutput
{
    [JsonProperty("creation_time")]
    public DateTime Created { get; set; }

    [JsonProperty("update_time")]
    public DateTime Updated { get; set; }

    [JsonProperty("user_id")]
    public long Id { get; set; }

    [JsonProperty("username")]
    public string Name { get; set; } = null!;

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("realname")]
    public string? RealName { get; set; }

    [JsonProperty("admin_role_in_auth")]
    public bool AdminRoleInAuth { get; set; }

    [JsonProperty("sysadmin_flag")]
    public bool SysAdminFlag { get; set; }
}
