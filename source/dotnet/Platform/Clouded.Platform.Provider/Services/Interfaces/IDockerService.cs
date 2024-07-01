using Docker.DotNet.Models;

namespace Clouded.Platform.Provider.Services.Interfaces;

public interface IDockerService
{
    public Task ImageBuild(
        string image,
        string gitRepositoryUrl,
        string gitRepositoryToken,
        string gitRepositoryBranch,
        Action<JSONMessage> progress,
        CancellationToken cancellationToken = default
    );

    public Task ImagePush(
        string image,
        Action<JSONMessage> progressCallback,
        CancellationToken cancellationToken = default
    );
}
