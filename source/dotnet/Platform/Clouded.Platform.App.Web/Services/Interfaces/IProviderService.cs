using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IProviderService<T, TInput> : IBaseService
    where T : ProviderEntity
{
    public Task<T?> CreateAsync(
        TInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task<T> UpdateAsync(
        TInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task DeleteAsync(
        T provider,
        HubConnection? providerHubConnection,
        ISnackbar snackbar,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );
    public HubConnection GetProviderHub(IStorageService storageService);
    public Task<FileStream> SelfDeploy(T authProvider);
    public Task Deploy(
        T adminProvider,
        HubConnection providerHubConnection,
        ISnackbar snackbar,
        CancellationToken cancellationToken
    );
    public Task Start(
        T adminProvider,
        HubConnection providerHubConnection,
        ISnackbar snackbar,
        CancellationToken cancellationToken
    );
    public Task Stop(
        T adminProvider,
        HubConnection providerHubConnection,
        ISnackbar snackbar,
        CancellationToken cancellationToken
    );
    public String GetEditRoute(string projectId, long? modelId);
    public String GetDetailRoute(string projectId, long? modelId);
    public String GetListRoute(string projectId);
}
