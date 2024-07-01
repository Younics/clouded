using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Provider.Services.Interfaces;

public interface IHubService
{
    public Task InitializeAsync(CancellationToken cancellationToken = default);

    public Task SendStatusUpdateAsync(
        string name,
        string userId,
        string regionCode,
        string? deployAt,
        long providerId,
        EProviderType providerType,
        EProviderStatus status,
        CancellationToken cancellationToken
    );
}
