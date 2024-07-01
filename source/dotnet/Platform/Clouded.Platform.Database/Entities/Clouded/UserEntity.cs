using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clouded.Platform.Database.Entities.Base;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("users")]
public class UserEntity : TrackableEntity
{
    [Required]
    [StringLength(200)]
    [Column("first_name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [Column("last_name")]
    public string LastName { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    [Required]
    [StringLength(256)]
    [Column("email")]
    public string Email { get; set; }

    [Required]
    [Column("password")]
    public string Password { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = null!;
    public virtual ICollection<ProjectUserRelationEntity> ProjectsRelation { get; set; } = null!;

    public virtual UserIntegrationEntity Integration { get; set; } = null!;
}
