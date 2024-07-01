using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth")]
public class AuthConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("api_key")]
    public string ApiKey { get; set; } = null!;

    [Required]
    [Column("documentation")]
    public bool Documentation { get; set; }

    [Required]
    [Column("management")]
    public bool Management { get; set; }

    [Required]
    [Column("management_token_key")]
    public string ManagementTokenKey { get; set; } = null!;

    [Required]
    [Column("management_identity_key")]
    public string ManagementIdentityKey { get; set; } = null!;

    [Required]
    [Column("management_password_key")]
    public string ManagementPasswordKey { get; set; } = null!;

    public virtual AuthHashConfigurationEntity Hash { get; set; } = null!;
    public virtual AuthCorsConfigurationEntity Cors { get; set; } = null!;

    public virtual AuthTokenConfigurationEntity Token { get; set; } = null!;
    public virtual AuthMailConfigurationEntity? Mail { get; set; }

    public virtual AuthIdentityOrganizationConfigurationEntity? IdentityOrganization { get; set; }

    public virtual AuthIdentityUserConfigurationEntity? IdentityUser { get; set; }

    public virtual AuthIdentityRoleConfigurationEntity? IdentityRole { get; set; }

    public virtual AuthIdentityPermissionConfigurationEntity? IdentityPermission { get; set; }

    public virtual ICollection<AuthSocialConfigurationEntity> SocialConfiguration { get; set; } =
        null!;

    public virtual ICollection<AuthUserAccessEntity> UserAccess { get; set; } = null!;

    [Required]
    [Column("provider_id")]
    public long ProviderId { get; set; }

    public virtual AuthProviderEntity Provider { get; protected set; } = null!;
}
