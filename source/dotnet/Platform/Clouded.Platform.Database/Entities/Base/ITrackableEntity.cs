namespace Clouded.Platform.Database.Entities.Base;

public interface ITrackableEntity
{
    DateTime Created { get; set; }

    DateTime? Updated { get; set; }
}
