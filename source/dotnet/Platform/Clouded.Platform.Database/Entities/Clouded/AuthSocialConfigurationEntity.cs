using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;
using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth_social")]
public class AuthSocialConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("type")]
    public ESocialAuthType Type { get; set; }

    [Required]
    [Column("key")]
    public string Key { get; set; }

    [Required]
    [Column("secret")]
    public string Secret { get; set; }

    [Required]
    [Column("redirect_url")]
    public string RedirectUrl { get; set; }

    [Required]
    [Column("denied_redirect_url")]
    public string DeniedRedirectUrl { get; set; }

    [Required]
    [Column("configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
