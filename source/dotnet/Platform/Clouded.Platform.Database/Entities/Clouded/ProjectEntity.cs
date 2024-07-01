using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("projects")]
public class ProjectEntity : TrackableEntity
{
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [StringLength(30)]
    [Column("code")]
    public string Code { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    public virtual ICollection<DomainEntity> Domains { get; set; } = null!;
    public virtual ICollection<DataSourceEntity> DataSources { get; set; } = null!;

    public virtual ICollection<AuthProviderEntity> AuthProviders { get; set; } = null!;

    public virtual ICollection<AdminProviderEntity> AdminProviders { get; set; } = null!;

    public virtual ICollection<FunctionProviderEntity> FunctionProviders { get; set; } = null!;
    public virtual ICollection<UserEntity> Users { get; set; } = null!;
    public virtual ICollection<ProjectUserRelationEntity> UsersRelation { get; set; } = null!;

    [Column("region_id")]
    public long RegionId { get; set; }
    public virtual RegionEntity Region { get; protected set; } = null!;
}
