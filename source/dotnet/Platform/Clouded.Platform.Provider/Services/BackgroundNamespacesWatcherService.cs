using Clouded.Platform.Models.Enums;
using Clouded.Platform.Provider.Services.Interfaces;
using k8s;
using k8s.Models;
using Serilog;

namespace Clouded.Platform.Provider.Services;

public class BackgroundNamespacesWatcherService(
    IHubService hubService,
    IKubernetesService kubernetesService
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Log.Information("Watching Namespaces...");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var watchNamespaces = kubernetesService.WatchProviderNamespacesAsync(
                    cancellationToken: cancellationToken
                );

                await foreach (
                    var (eventType, @namespace) in watchNamespaces.WithCancellation(
                        cancellationToken
                    )
                )
                    await OnNamespaceChanged(eventType, @namespace, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error watching namespaces");
            }

            Log.Information("Retrying namespaces watching...");
        }
    }

    private async Task OnNamespaceChanged(
        WatchEventType eventType,
        V1Namespace @namespace,
        CancellationToken cancellationToken
    )
    {
        Log.Information("[{@Namespace}]: Namespace {EventType}", @namespace.Name(), eventType);

        switch (eventType)
        {
            case WatchEventType.Modified:
                var status = @namespace.Status.Phase;
                if (status == "Terminating")
                {
                    Log.Information(
                        "[{@Namespace}]: Namespace {EventType} - Status: {Status}...",
                        @namespace.Name(),
                        eventType,
                        status
                    );

                    await SendStatusUpdate(
                        @namespace,
                        EProviderStatus.Terminating,
                        cancellationToken
                    );
                }

                break;
            case WatchEventType.Deleted:
                Log.Information(
                    "[{@Namespace}]: Namespace {EventType} - Status: {Status}!",
                    @namespace.Name(),
                    eventType,
                    EProviderStatus.Terminated
                );

                await SendStatusUpdate(@namespace, EProviderStatus.Terminated, cancellationToken);
                break;
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
