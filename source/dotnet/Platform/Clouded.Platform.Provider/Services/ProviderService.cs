using Clouded.Platform.Models.Dtos.Provider;
using Clouded.Platform.Models.Enums;
using Clouded.Platform.Provider.Options;
using Clouded.Platform.Provider.Services.Interfaces;
using Clouded.Shared;
using k8s.Models;
using Serilog;

namespace Clouded.Platform.Provider.Services;

public class ProviderService(
    ApplicationOptions options,
    IHubService hubService,
    IDockerService dockerService,
    IKubernetesService kubernetesService
) : IProviderService
{
    private readonly CloudedOptions _options = options.Clouded;

    public async Task CreateAsync(
        ProviderCreateInput input,
        CancellationToken cancellationToken = default
    )
    {
        await kubernetesService.CreateAsync(
            input.Name,
            input.Id.ToString(),
            input.Type.GetEnumName(),
            input.UserId,
            input.HubRegionCode.GetEnumName(),
            input.Type switch
            {
                EProviderType.Auth => _options.Provider.Docker.AuthProviderImage,
                EProviderType.Admin => _options.Provider.Docker.AdminProviderImage,
                EProviderType.Api => _options.Provider.Docker.ApiProviderImage,
                EProviderType.Function => input.CustomImage!,
                _ => throw new NotSupportedException()
            },
            input.DeployAt,
            "/health/liveness",
            "/health/readiness",
            input.Envs.Select(x => new V1EnvVar(x.Key, x.Value)),
            cancellationToken
        );
    }

    public async Task StartAsync(
        string name,
        Dictionary<string, string>? envs,
        CancellationToken cancellationToken = default
    )
    {
        await kubernetesService.UpdateDeploymentAsync(
            name,
            replicas: 1,
            containerEnvs: envs?.Select(x => new V1EnvVar(x.Key, x.Value)),
            cancellationToken
        );
    }

    public async Task StopAsync(string name, CancellationToken cancellationToken = default)
    {
        await kubernetesService.StopAsync(name, cancellationToken);
    }

    public async Task DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        await kubernetesService.DeleteAsync(name, cancellationToken);
    }

    public async Task FunctionBuildAndPush(
        FunctionBuildInput input,
        CancellationToken cancellationToken = default
    )
    {
        var previousStatus = await kubernetesService.GetPodStatus(input.Name, cancellationToken);

        await hubService.SendStatusUpdateAsync(
            input.Name,
            input.UserId,
            input.HubRegionCode.GetEnumName(),
            input.DeployAt.ToString("O"),
            input.Id,
            EProviderType.Function,
            EProviderStatus.Building,
            cancellationToken
        );

        await dockerService.ImageBuild(
            input.Image,
            input.GitRepositoryUrl,
            input.GitRepositoryToken,
            input.GitRepositoryBranch,
            message =>
            {
                Log.Information("[FunctionImageBuild]: ${@Message}", message);
            },
            cancellationToken
        );

        await dockerService.ImagePush(
            input.Image,
            message =>
            {
                Log.Information("[FunctionImagePush]: ${@Message}", message);
            },
            cancellationToken
        );

        if (previousStatus == EProviderStatus.Running)
        {
            // Refactor to deploy restart
            await StopAsync(input.Name, cancellationToken);
            await StartAsync(input.Name, null, cancellationToken);
        }
        else
        {
            await hubService.SendStatusUpdateAsync(
                input.Name,
                input.UserId,
                input.HubRegionCode.GetEnumName(),
                input.DeployAt.ToString("O"),
                input.Id,
                EProviderType.Function,
                previousStatus,
                cancellationToken
            );
        }
    }
}
