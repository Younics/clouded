using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth_token")]
public class AuthTokenConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("valid_issuer")]
    public string ValidIssuer { get; set; }

    [Required]
    [Column("validate_issuer")]
    public bool ValidateIssuer { get; set; } = true;

    [Required]
    [Column("validate_audience")]
    public bool ValidateAudience { get; set; } = true;

    [Required]
    [Column("validate_issuer_signing_key")]
    public bool ValidateIssuerSigningKey { get; set; } = true;

    [Required]
    [Column("secret")]
    public string Secret { get; set; }

    [Required]
    [Column("access_token_expiration")]
    public int AccessTokenExpiration { get; set; } = 180;

    [Required]
    [Column("refresh_token_expiration")]
    public int RefreshTokenExpiration { get; set; } = 432000;

    [Required]
    [Column("configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
