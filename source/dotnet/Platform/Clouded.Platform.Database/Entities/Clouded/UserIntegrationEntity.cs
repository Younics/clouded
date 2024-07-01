using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("user_integrations")]
public class UserIntegrationEntity : TrackableEntity
{
    [Column("harbor_user")]
    public string HarborUser { get; set; } = null!;

    [Column("harbor_password")]
    public string HarborPassword { get; set; } = null!;

    [Column("harbor_project")]
    public string HarborProject { get; set; } = null!;

    [Column("github_oauth_token")]
    public string? GithubOauthToken { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    public virtual UserEntity User { get; protected set; } = null!;
}
