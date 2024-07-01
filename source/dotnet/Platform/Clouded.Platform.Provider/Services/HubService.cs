using Clouded.Platform.Models.Dtos.Hub;
using Clouded.Platform.Models.Enums;
using Clouded.Platform.Provider.Options;
using Clouded.Platform.Provider.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Clouded.Platform.Provider.Services;

public class HubService(ApplicationOptions options) : IHubService
{
    private readonly Dictionary<string, EProviderStatus> _statusCache = new();
    private readonly Dictionary<string, HubConnection> _regionHubs = new();
    private readonly HubOptions _options = options.Clouded.Hub;

    private bool _isInitialized;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
            return;

        foreach (var region in _options.RegionServerUrls)
        {
            var providerHub = new HubConnectionBuilder()
                .WithUrl(
                    new Uri(new Uri(region.Value), "/hubs/provider"),
                    options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(_options.ApiKey)!;
                    }
                )
                .WithAutomaticReconnect(
                    new[]
                    {
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(60),
                        TimeSpan.FromSeconds(120),
                        TimeSpan.FromSeconds(300)
                    }
                )
                .Build();

            _regionHubs[region.Key] = providerHub;

            await providerHub.StartAsync(cancellationToken);
        }

        _isInitialized = true;
    }

    public async Task SendStatusUpdateAsync(
        string name,
        string userId,
        string regionCode,
        string? deployAt,
        long providerId,
        EProviderType providerType,
        EProviderStatus status,
        CancellationToken cancellationToken
    )
    {
        var statusKey = GetRegionKey(regionCode, name);
        var previousStatus = GetStatus(statusKey);

        if (previousStatus == status)
            return;

        var input = new HubProviderUpdateStatus
        {
            Id = providerId,
            UserId = userId,
            Type = providerType,
            Status = status,
            DeployAt =
                deployAt != null ? DateTime.ParseExact(deployAt, "O", null).ToUniversalTime() : null
        };

        await _regionHubs[regionCode].SendAsync("UpdateStatus", input, cancellationToken);

        _statusCache[statusKey] = status;
    }

    private EProviderStatus? GetStatus(string key)
    {
        _statusCache.TryGetValue(key, out var status);
        return status;
    }

    private static string GetRegionKey(string regionCode, string name) => $"{regionCode}_{name}";
}
