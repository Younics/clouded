using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth_cors")]
public class AuthCorsConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("allowed_methods")]
    public string AllowedMethods { get; set; } = "*";

    [Required]
    [Column("allowed_origins")]
    public string AllowedOrigins { get; set; } = "*";

    [Required]
    [Column("allowed_headers")]
    public string AllowedHeaders { get; set; } = "*";

    [Column("exposed_headers")]
    public string? ExposedHeaders { get; set; }

    [Required]
    [Column("supports_credentials")]
    public bool SupportsCredentials { get; set; }

    [Required]
    [Column("max_age")]
    public int MaxAge { get; set; }

    [Required]
    [Column("configuration_id")]
    public long ConfigurationId { get; set; }

    public virtual AuthConfigurationEntity Configuration { get; protected set; } = null!;
}
