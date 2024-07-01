using Clouded.Platform.Models.Dtos.Provider;

namespace Clouded.Platform.Provider.Services.Interfaces;

public interface IProviderService
{
    public Task CreateAsync(
        ProviderCreateInput input,
        CancellationToken cancellationToken = default
    );

    public Task StartAsync(
        string name,
        Dictionary<string, string>? envs,
        CancellationToken cancellationToken = default
    );

    public Task StopAsync(string name, CancellationToken cancellationToken = default);

    public Task DeleteAsync(string name, CancellationToken cancellationToken = default);

    public Task FunctionBuildAndPush(
        FunctionBuildInput input,
        CancellationToken cancellationToken = default
    );
}
