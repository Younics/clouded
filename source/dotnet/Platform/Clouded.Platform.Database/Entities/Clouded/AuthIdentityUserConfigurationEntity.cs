using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Clouded;

public class AuthIdentityUserConfigurationEntity : AuthIdentityConfigurationEntity
{
    [Required]
    [Column("column_password")]
    public string ColumnPassword { get; set; }

    [Required]
    [Column("user_configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
