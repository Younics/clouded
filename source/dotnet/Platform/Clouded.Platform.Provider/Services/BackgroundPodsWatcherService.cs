using Clouded.Platform.Models.Enums;
using Clouded.Platform.Provider.Services.Interfaces;
using k8s;
using k8s.Models;
using Serilog;

namespace Clouded.Platform.Provider.Services;

public class BackgroundPodsWatcherService(
    IHubService hubService,
    IKubernetesService kubernetesService
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Log.Information("Watching Pods...");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var watchPods = kubernetesService.WatchNamespacedPodsAsync(cancellationToken);

                await foreach (
                    var (podEventType, pod) in watchPods.WithCancellation(cancellationToken)
                )
                    await OnPodChanged(podEventType, pod, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error watching pods");
            }

            Log.Information("Retrying pods watching...");
        }
    }

    private async Task OnPodChanged(
        WatchEventType eventType,
        V1Pod pod,
        CancellationToken cancellationToken
    )
    {
        var @namespace = await kubernetesService.GetNamespace(pod.Namespace());
        EProviderStatus? status = null;

        var starting =
            pod.Metadata.CreationTimestamp != null && pod.Metadata.DeletionTimestamp == null;

        var stopping =
            pod.Metadata.CreationTimestamp != null && pod.Metadata.DeletionTimestamp != null;

        if (starting)
            status = EProviderStatus.Starting;

        if (pod.Status.ContainerStatuses != null)
        {
            var running = pod.Status.ContainerStatuses.All(x => x.Ready);

            var stopped = stopping && !running;

            if (stopped)
                status = EProviderStatus.Stopped;
            else if (stopping)
                status = EProviderStatus.Stopping;
            else if (running)
                status = EProviderStatus.Running;
        }

        if (status.HasValue)
        {
            Log.Information(
                "[{@Namespace}|{Pod}]: Pod status: {Status}",
                @namespace.Name(),
                pod.Name(),
                status.Value
            );

            await SendStatusUpdate(@namespace, status.Value, cancellationToken);
        }
    }

    private async Task SendStatusUpdate(
        V1Namespace @namespace,
        EProviderStatus status,
        CancellationToken cancellationToken
    )
    {
        var deployment = await kubernetesService.GetDeployment(@namespace.Name());
        await hubService.SendStatusUpdateAsync(
            @namespace.Name(),
            @namespace.Metadata.Labels[KubernetesService.UserIdLabel],
            @namespace.Metadata.Labels[KubernetesService.HubRegionCodeLabel],
            deployment != null
            && deployment.Metadata.Annotations.ContainsKey(KubernetesService.DeployAtLabel)
                ? deployment.Metadata.Annotations[KubernetesService.DeployAtLabel]
                : null,
            long.Parse(@namespace.Metadata.Labels[KubernetesService.ProviderIdLabel]),
            (EProviderType)
                Enum.Parse(
                    typeof(EProviderType),
                    @namespace.Metadata.Labels[KubernetesService.ProviderTypeLabel]
                ),
            status,
            cancellationToken
        );
    }
}
