using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth_user_access")]
public class AuthUserAccessEntity : TrackableEntity
{
    [Required]
    [Column("identity")]
    public string Identity { get; set; } = "*";

    [Required]
    [Column("password")]
    public string Password { get; set; } = "*";

    [Required]
    [Column("configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
