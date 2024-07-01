using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("provider_auth_function_relation")]
public class AuthProviderFunctionRelationEntity
{
    [Column("index")]
    public int Index { get; set; }

    [Column("auth_provider_id")]
    public long AuthProviderId { get; set; }

    public virtual AuthProviderEntity AuthProvider { get; protected set; } = null!;

    [Column("function_id")]
    public long FunctionId { get; set; }

    public virtual FunctionEntity Function { get; protected set; } = null!;
}
