using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("permissions")]
public class PermissionEntity : Entity
{
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; }
}
