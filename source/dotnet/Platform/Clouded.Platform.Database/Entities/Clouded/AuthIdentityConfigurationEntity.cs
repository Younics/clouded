using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;
using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("configurations_auth_identity")]
public abstract class AuthIdentityConfigurationEntity : TrackableEntity
{
    [Required]
    [Column("identity_type")]
    public EIdentityType IdentityType { get; set; }

    [Required]
    [Column("schema")]
    public string Schema { get; set; }

    [Required]
    [Column("table")]
    public string Table { get; set; }

    [Required]
    [Column("column_id")]
    public string ColumnId { get; set; }

    [Required]
    [Column("column_identity")]
    public string ColumnIdentity { get; set; }
}
