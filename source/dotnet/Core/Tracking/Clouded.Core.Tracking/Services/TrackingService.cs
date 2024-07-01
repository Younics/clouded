using Clouded.Core.Tracking.Options;
using Clouded.Core.Tracking.Services.Interfaces;
using Microsoft.Extensions.Options;
using Mixpanel;

namespace Clouded.Core.Tracking.Services;

public class TrackingService : ITrackingService
{
    private readonly TrackingOptions _options;
    private readonly MixpanelClient _client;

    public TrackingService(IOptions<TrackingOptions> options)
    {
        _options = options.Value;
        _client = new MixpanelClient(_options.ApiKey);
    }

    public async Task TrackAsync(
        string eventName,
        Dictionary<string, object?>? properties = null,
        CancellationToken cancellationToken = default
    ) => await _client.TrackAsync(eventName, EnhanceProperties(properties), cancellationToken);

    public async Task TrackUserAsync(
        object userId,
        string eventName,
        Dictionary<string, object?>? properties = null,
        CancellationToken cancellationToken = default
    ) =>
        await _client.TrackAsync(
            eventName,
            userId,
            EnhanceProperties(properties),
            cancellationToken
        );

    public async Task CreateUserAsync(
        object userId,
        Dictionary<string, object?>? properties = null,
        CancellationToken cancellationToken = default
    ) => await _client.PeopleSetAsync(userId, EnhanceProperties(properties), cancellationToken);

    private object EnhanceProperties(Dictionary<string, object?>? properties)
    {
        properties ??= new Dictionary<string, object?>();
        properties["Environment"] = _options.Environment;
        return properties;
    }
}
