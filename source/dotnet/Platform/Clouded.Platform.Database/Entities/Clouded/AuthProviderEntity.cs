using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Clouded;

public class AuthProviderEntity : ProviderEntity
{
    [Required]
    [Column("datasource_id")]
    public long DataSourceId { get; set; }
    public virtual DataSourceEntity DataSource { get; set; } = null!;

    public virtual ICollection<FunctionEntity> Hooks { get; set; } = null!;
    public virtual ICollection<AuthProviderFunctionRelationEntity> HooksRelation { get; set; } =
        null!;

    public virtual AuthConfigurationEntity Configuration { get; set; } = null!;
}
