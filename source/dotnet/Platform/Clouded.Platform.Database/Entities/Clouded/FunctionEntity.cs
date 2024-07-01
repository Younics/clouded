using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Function.Shared;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_function_hooks")]
public class FunctionEntity : TrackableEntity
{
    [Required]
    [Column("executable_name")]
    public string ExecutableName { get; set; } = null!;

    [Required]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required]
    [Column("type")]
    public EFunctionType Type { get; set; }

    [Required]
    [Column("provider_id")]
    public long ProviderId { get; set; }

    public virtual FunctionProviderEntity Provider { get; protected set; } = null!;

    public virtual ICollection<AuthProviderEntity> UsingAuthProviders { get; set; } = null!;
    public virtual ICollection<AuthProviderFunctionRelationEntity> UsingAuthProvidersRelation { get; set; } =
        null!;

    public virtual ICollection<AdminProviderEntity> UsingAdminProviders { get; set; } = null!;
    public virtual ICollection<AdminProviderFunctionRelationEntity> UsingAdminProvidersRelation { get; set; } =
        null!;

    public virtual ICollection<AdminTablesConfigurationEntity> UsingAdminProviderTables { get; set; } =
        null!;
    public virtual ICollection<AdminProviderTableFunctionRelationEntity> UsingAdminProviderTablesRelation { get; set; } =
        null!;
}
