using Clouded.Platform.Provider.Options;
using Clouded.Platform.Provider.Services.Interfaces;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Clouded.Platform.Provider.Services;

public class DockerService : IDockerService
{
    private readonly CloudedOptions _options;
    private readonly AuthConfig _authConfig;
    private readonly DockerClient _docker;

    public DockerService(ApplicationOptions options)
    {
        _options = options.Clouded;
        _authConfig = new AuthConfig
        {
            ServerAddress = _options.Harbor.Server,
            Username = _options.Harbor.User,
            Password = _options.Harbor.Password
        };

        _docker = new DockerClientConfiguration().CreateClient();
    }

    public async Task ImageBuild(
        string image,
        string gitRepositoryUrl,
        string gitRepositoryToken,
        string gitRepositoryBranch,
        Action<JSONMessage> progress,
        CancellationToken cancellationToken = default
    )
    {
        var contextPath = Path.Join(
            Environment.CurrentDirectory,
            "Assets",
            "Docker",
            "prebuilt_function.tar.gz"
        );
        await using var contextStream = new FileStream(contextPath, FileMode.Open);

        var imageTagLatest = $"{image}:latest";

        await _docker.Images.BuildImageFromDockerfileAsync(
            new ImageBuildParameters
            {
                Dockerfile = "Dockerfile",
                Tags = new[]
                {
                    imageTagLatest,
                    // TODO: Version tag for rollback to older version
                },
                NoCache = true,
                BuildArgs = new Dictionary<string, string>
                {
                    ["GIT_REPOSITORY_URL"] = gitRepositoryUrl,
                    ["GIT_REPOSITORY_TOKEN"] = gitRepositoryToken,
                    ["GIT_REPOSITORY_BRANCH"] = gitRepositoryBranch,
                    ["NUGET_REPOSITORY_USERNAME"] = _options.Nuget.User,
                    ["NUGET_REPOSITORY_PASSWORD"] = _options.Nuget.Password
                }
            },
            contextStream,
            new[] { _authConfig },
            null,
            new Progress<JSONMessage>(progress),
            cancellationToken
        );
    }

    public async Task ImagePush(
        string image,
        Action<JSONMessage> progress,
        CancellationToken cancellationToken = default
    )
    {
        await _docker.Images.PushImageAsync(
            image,
            new ImagePushParameters(),
            _authConfig,
            new Progress<JSONMessage>(progress),
            cancellationToken
        );
    }
}
