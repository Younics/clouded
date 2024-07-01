using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_admin_tables")]
public class AdminTablesConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("schema")]
    public string Schema { get; set; } = null!;

    [Required]
    [Column("table")]
    public string Table { get; set; } = null!;

    [Required]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("singular_name")]
    public string? SingularName { get; set; }

    [Required]
    [Column("in_menu")]
    public bool InMenu { get; set; }

    [Column("nav_group")]
    public string? NavGroup { get; set; }

    [Column("icon")]
    public string? Icon { get; set; }

    [Column("columns")]
    public string Columns { get; set; } = null!;

    [Column("virtual_columns")]
    public string VirtualColumns { get; set; } = null!;

    [Required]
    [Column("provider_id")]
    public long ProviderId { get; set; }

    public virtual AdminProviderEntity Provider { get; protected set; } = null!;

    [Required]
    [Column("data_source_id")]
    public long DataSourceId { get; set; }

    public virtual DataSourceEntity DataSource { get; protected set; } = null!;

    public virtual ICollection<FunctionEntity> Functions { get; set; } = null!;
    public virtual ICollection<AdminProviderTableFunctionRelationEntity> FunctionsRelation { get; set; } =
        null!;
}
