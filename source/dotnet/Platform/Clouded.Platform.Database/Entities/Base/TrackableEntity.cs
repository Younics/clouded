using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Base;

public abstract class TrackableEntity : Entity, ITrackableEntity
{
    [Column("created")]
    public DateTime Created { get; set; }

    [Column("updated")]
    public DateTime? Updated { get; set; }
}
