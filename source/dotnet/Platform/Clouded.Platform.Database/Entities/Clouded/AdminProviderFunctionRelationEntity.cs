using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Shared.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("provider_admin_function_relation")]
public class AdminProviderFunctionRelationEntity
{
    [Required]
    [Column("operation_type")]
    public EAdminProviderFunctionTrigger OperationType { get; set; }

    [Required]
    [Column("admin_provider_id")]
    public long AdminProviderId { get; set; }

    public virtual AdminProviderEntity AdminProvider { get; protected set; } = null!;

    [Required]
    [Column("function_id")]
    public long FunctionId { get; set; }

    public virtual FunctionEntity Function { get; protected set; } = null!;
}
