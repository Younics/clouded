using System.IO.Compression;
using System.Text;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace Clouded.Platform.App.Web.Services;

public abstract class ProviderService<T>(
    ApplicationOptions options,
    IWebHostEnvironment environment
)
    where T : ProviderEntity
{
    protected readonly DatabaseOptions DatabaseOptions = options.Clouded.Database;
    protected readonly IWebHostEnvironment Environment = environment;
    protected readonly CloudedOptions CloudedOptions = options.Clouded;

    public async Task Deploy(
        T provider,
        HubConnection providerHubConnection,
        ISnackbar snackbar,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await providerHubConnection.InvokeAsync(
                "Create",
                provider.Id,
                provider.Type,
                cancellationToken
            );
        }
        catch (HubException e)
        {
            if (e.Message.Contains("is unauthorized"))
            {
                snackbar.Add(
                    "Your account is not authorized. Please try to re-login.",
                    Severity.Error
                );
            }
        }
    }

    public async Task Start(
        T provider,
        HubConnection providerHubConnection,
        ISnackbar snackbar,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await providerHubConnection.InvokeAsync(
                "Start",
                provider.Id,
                provider.Type,
                cancellationToken
            );
        }
        catch (HubException e)
        {
            if (e.Message.Contains("is unauthorized"))
            {
                snackbar.Add(
                    "Your account is not authorized. Please try to re-login.",
                    Severity.Error
                );
            }
        }
    }

    public async Task Stop(
        T provider,
        HubConnection providerHubConnection,
        ISnackbar snackbar,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await providerHubConnection.InvokeAsync(
                "Stop",
                provider.Id,
                provider.Type,
                cancellationToken
            );
        }
        catch (HubException e)
        {
            if (e.Message.Contains("is unauthorized"))
            {
                snackbar.Add(
                    "Your account is not authorized. Please try to re-login.",
                    Severity.Error
                );
            }
        }
    }

    public HubConnection GetProviderHub(IStorageService storageService)
    {
        return new HubConnectionBuilder()
            .WithUrl(
                new Uri(new Uri(CloudedOptions.Hub.ServerUrl), "/hubs/provider"),
                options =>
                {
                    options.AccessTokenProvider = async () => await storageService.GetAccessToken();
                }
            )
            .WithAutomaticReconnect()
            .Build();
    }

    protected async Task<FileStream> GetSelfDeployZip(
        Dictionary<string, string> envs,
        string providerImageRepository
    )
    {
        var tmpFolder = Guid.NewGuid().ToString();
        var tmpDeploys = Path.Combine(Path.GetTempPath(), "tmp_deploys");
        var dockerDeployRunScriptPath = Path.Combine(
            Environment.ContentRootPath,
            "Assets",
            "Deploy",
            "docker",
            "run.sh"
        );
        var tmpDeployDockerRunScriptPath = Path.Combine(tmpDeploys, tmpFolder, "docker", "run.sh");
        var tmpDeployFolderPath = Path.Combine(tmpDeploys, tmpFolder);
        var tmpDeployDockerFolderPath = Path.Combine(tmpDeployFolderPath, "docker");
        var tmpDeployHelmFolderPath = Path.Combine(tmpDeployFolderPath, "helm");
        Directory.CreateDirectory(tmpDeployDockerFolderPath);
        Directory.CreateDirectory(tmpDeployHelmFolderPath);

        await File.WriteAllLinesAsync(
            Path.Combine(tmpDeployDockerFolderPath, "env"),
            envs.Select(x => $"{x.Key}={x.Value}").ToArray(),
            Encoding.UTF8
        );

        File.Copy(dockerDeployRunScriptPath, tmpDeployDockerRunScriptPath);
        //todo copy helm
        // File.Copy(dockerDeployRunScriptPath, tmpDeployDockerRunScriptPath);

        var dockerRunText = await File.ReadAllTextAsync(
            Path.Combine(tmpDeployDockerFolderPath, "run.sh")
        );
        dockerRunText = dockerRunText.Replace("$DOCKER_IMAGE", providerImageRepository);

        await File.WriteAllTextAsync(
            Path.Combine(tmpDeployDockerFolderPath, "run.sh"),
            dockerRunText,
            Encoding.UTF8
        );

        ZipFile.CreateFromDirectory(
            tmpDeployFolderPath,
            Path.Combine(tmpDeploys, $"{tmpFolder}.zip")
        );

        return File.OpenRead(Path.Combine(tmpDeploys, $"{tmpFolder}.zip"));
    }
}
