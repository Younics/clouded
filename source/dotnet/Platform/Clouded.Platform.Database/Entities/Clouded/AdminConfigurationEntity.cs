using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_admin")]
public class AdminConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("token_key")]
    public string TokenKey { get; set; } = null!;

    [Required]
    [Column("identity_key")]
    public string IdentityKey { get; set; } = null!;

    [Required]
    [Column("password_key")]
    public string PasswordKey { get; set; } = null!;

    [Required]
    [Column("provider_id")]
    public long ProviderId { get; set; }

    public virtual AdminProviderEntity Provider { get; protected set; } = null!;
}
