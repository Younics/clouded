using Clouded.Platform.Models.Enums;
using k8s;
using k8s.Models;

namespace Clouded.Platform.Provider.Services.Interfaces;

public interface IKubernetesService
{
    public Task<V1Namespace?> GetNamespace(
        string @namespace,
        CancellationToken cancellationToken = default
    );

    public Task<V1Deployment?> GetDeployment(
        string @namespace,
        CancellationToken cancellationToken = default
    );

    public Task<V1Pod?> GetPod(string @namespace, CancellationToken cancellationToken = default);

    public Task<EProviderStatus> GetPodStatus(
        string @namespace,
        CancellationToken cancellationToken = default
    );

    public IAsyncEnumerable<(
        WatchEventType EventType,
        V1Namespace Namespace
    )> WatchProviderNamespacesAsync(CancellationToken cancellationToken = default);

    public IAsyncEnumerable<(WatchEventType EventType, V1Pod Pod)> WatchNamespacedPodsAsync(
        CancellationToken cancellationToken = default
    );

    public Task CreateAsync(
        string name,
        string providerId,
        string providerType,
        string userId,
        string hubRegionId,
        string image,
        DateTime deployAt,
        string livenessUrl,
        string readinessUrl,
        IEnumerable<V1EnvVar> envs,
        CancellationToken cancellationToken = default
    );

    public Task StartAsync(string name, CancellationToken cancellationToken = default);

    public Task StopAsync(string name, CancellationToken cancellationToken = default);

    public Task DeleteAsync(string name, CancellationToken cancellationToken = default);

    public Task UpdateDeploymentAsync(
        string name,
        int? replicas = null,
        IEnumerable<V1EnvVar>? containerEnvs = null,
        CancellationToken cancellationToken = default
    );
}
