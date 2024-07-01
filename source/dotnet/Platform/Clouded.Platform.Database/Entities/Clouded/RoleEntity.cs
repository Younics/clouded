using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("roles")]
public class RoleEntity : Entity
{
    [StringLength(200)]
    [Column("name")]
    public string Name { get; set; } = null!;
}
