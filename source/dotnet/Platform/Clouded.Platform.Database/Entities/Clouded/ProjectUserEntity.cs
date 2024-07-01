using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Clouded;

[Table("project_user_relation")]
public class ProjectUserRelationEntity
{
    [Column("project_id")]
    public long ProjectId { get; set; }

    public virtual ProjectEntity Project { get; protected set; } = null!;

    [Column("user_id")]
    public long UserId { get; set; }

    public virtual UserEntity User { get; protected set; } = null!;
}
