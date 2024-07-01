using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("admin_provider_datasource_relation")]
public class AdminProviderDataSourceRelationEntity
{
    [Column("has_user_settings_table")]
    public bool HasUserSettingsTable { get; set; }

    [Column("user_settings_schema")]
    public string? UserSettingsSchema { get; set; }

    [Column("admin_provider_id")]
    public long AdminProviderId { get; set; }

    public virtual AdminProviderEntity AdminProvider { get; protected set; } = null!;

    [Column("datasource_id")]
    public long DataSourceId { get; set; }

    public virtual DataSourceEntity DataSource { get; protected set; } = null!;
}
