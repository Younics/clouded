using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("datasources")]
public class DataSourceEntity : TrackableEntity
{
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = null!;

    public virtual DataSourceConfigurationEntity Configuration { get; set; } = null!;

    [Required]
    [Column("project_id")]
    public long ProjectId { get; set; }

    public virtual ProjectEntity Project { get; protected set; } = null!;

    public virtual ICollection<AdminProviderEntity> UsingAdminProviders { get; set; } = null!;
    public virtual ICollection<AdminTablesConfigurationEntity> UsingAdminTablesConfiguration { get; set; } =
        null!;
    public virtual ICollection<AdminProviderDataSourceRelationEntity> UsingAdminProvidersRelation { get; set; } =
        null!;
}
