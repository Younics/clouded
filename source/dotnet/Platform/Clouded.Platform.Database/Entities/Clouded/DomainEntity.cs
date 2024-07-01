using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("domains")]
public class DomainEntity : TrackableEntity
{
    [Column("value")]
    public string Value { get; set; }

    [Required]
    [Column("project_id")]
    public long ProjectId { get; set; }
    public virtual ProjectEntity Project { get; protected set; } = null!;
    public virtual ICollection<AuthProviderEntity> AuthProviders { get; set; } = null!;
    public virtual ICollection<AdminProviderEntity> AdminProviders { get; set; } = null!;
    public virtual ICollection<FunctionProviderEntity> FunctionProviders { get; set; } = null!;
}
