using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Shared.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("provider_admin_table_function_relation")]
public class AdminProviderTableFunctionRelationEntity
{
    [Required]
    [Column("operation_type")]
    public EAdminProviderFunctionTrigger OperationType { get; set; }

    [Required]
    [Column("admin_provider_table_id")]
    public long AdminProviderTableId { get; set; }

    public virtual AdminTablesConfigurationEntity AdminProviderTable { get; protected set; } =
        null!;

    [Required]
    [Column("function_id")]
    public long FunctionId { get; set; }

    public virtual FunctionEntity Function { get; protected set; } = null!;
}
