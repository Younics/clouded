using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_admin_user_access")]
public class AdminUserAccessConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("identity")]
    public string Identity { get; set; } = null!;

    [Required]
    [Column("password")]
    public string Password { get; set; } = null!;

    [Required]
    [Column("provider_id")]
    public long ProviderId { get; set; }

    public virtual AdminProviderEntity Provider { get; protected set; } = null!;
}
