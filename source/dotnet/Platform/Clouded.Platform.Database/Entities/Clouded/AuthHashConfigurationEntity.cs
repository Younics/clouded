using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;
using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth_hash")]
public abstract class AuthHashConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("algorithm_type")]
    public EHashType AlgorithmType { get; set; }

    [Required]
    [Column("secret")]
    public string Secret { get; set; }

    [Required]
    [Column("configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
