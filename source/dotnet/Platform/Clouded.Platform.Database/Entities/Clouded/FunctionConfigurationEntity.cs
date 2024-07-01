using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;
using Clouded.Shared.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_function")]
public class FunctionConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("api_key")]
    public string ApiKey { get; set; } = null!;

    [Required]
    [Column("repository_id")]
    public string RepositoryId { get; set; } = null!;

    [Required]
    [Column("branch")]
    public string Branch { get; set; } = "master";

    [Required]
    [Column("repository_type")]
    public RepositoryType RepositoryType { get; set; }

    [Required]
    [Column("provider_id")]
    public long ProviderId { get; set; }

    public virtual FunctionProviderEntity Provider { get; protected set; } = null!;
}
