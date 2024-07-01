using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Core.DataSource.Shared;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_datasource")]
public class DataSourceConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("type")]
    public DatabaseType Type { get; set; }

    [Required]
    [Column("server")]
    public string Server { get; set; } = null!;

    [Required]
    [Column("port")]
    public int Port { get; set; }

    [Required]
    [Column("username")]
    public string Username { get; set; } = null!;

    [Required]
    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("database")]
    public string Database { get; set; } = null!;

    [Required]
    [Column("datasource_id")]
    public long DataSourceId { get; set; }

    public virtual DataSourceEntity DataSource { get; protected set; } = null!;
}
