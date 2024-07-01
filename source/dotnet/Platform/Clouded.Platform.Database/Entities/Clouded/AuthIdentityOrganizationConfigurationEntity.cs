using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Clouded;

public class AuthIdentityOrganizationConfigurationEntity : AuthIdentityConfigurationEntity
{
    [Required]
    [Column("organization_configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
