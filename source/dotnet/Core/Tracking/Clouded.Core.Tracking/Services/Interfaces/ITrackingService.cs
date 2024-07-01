namespace Clouded.Core.Tracking.Services.Interfaces;

public interface ITrackingService
{
    public Task TrackAsync(
        string eventName,
        Dictionary<string, object?>? eventProperties = null,
        CancellationToken cancellationToken = default
    );

    public Task TrackUserAsync(
        object userId,
        string eventName,
        Dictionary<string, object?>? properties = null,
        CancellationToken cancellationToken = default
    );

    public Task CreateUserAsync(
        object userId,
        Dictionary<string, object?>? properties = null,
        CancellationToken cancellationToken = default
    );
}
