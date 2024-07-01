using System.Text;
using Blazored.SessionStorage;
using Clouded.Function.Shared;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Exceptions;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Shared;
using Clouded.Shared.Enums;
using Flurl;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Octokit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using RepositoryType = Clouded.Shared.Enums.RepositoryType;

namespace Clouded.Platform.App.Web.Helpers;

public class GithubRepositoryHandlerDefinition : RepositoryHandlerDefinition
{
    private GitHubClient _gitHubClient;

    public GithubRepositoryHandlerDefinition(
        UserIntegrationEntity currentUserIntegration,
        CloudedOptions cloudedOptions,
        CloudedDbContext context,
        ISessionStorageService sessionStorageService,
        IUserIntegrationService userIntegrationService
    )
        : base(
            ERepositoryProvider.Github.GetEnumName(),
            Icons.Custom.Brands.GitHub,
            currentUserIntegration,
            cloudedOptions,
            context,
            sessionStorageService,
            userIntegrationService
        )
    {
        if (currentUserIntegration.GithubOauthToken == null)
        {
            _gitHubClient = new(new ProductHeaderValue("Clouded"));
        }
        else
        {
            _gitHubClient = new(new ProductHeaderValue("Clouded"))
            {
                Credentials = new Credentials(currentUserIntegration.GithubOauthToken)
            };
        }
    }

    public override bool IsRepositoryProviderConnected()
    {
        return CurrentUserIntegration?.GithubOauthToken != null;
    }

    public override async Task Process(string uriQuery, CancellationToken cancellationToken)
    {
        var queryParameters = QueryHelpers.ParseQuery(uriQuery);

        await Validate(
            queryParameters.GetValueOrDefault("code"),
            queryParameters.GetValueOrDefault("state"),
            cancellationToken
        );
    }

    public override async Task Connect(
        NavigationManager navigationManager,
        string ProjectId,
        CancellationToken cancellationToken
    )
    {
        var csrf = Generator.RandomString(24);
        await SessionStorageService.SetItemAsync(
            "RepositoryType",
            RepositoryType.Github,
            cancellationToken
        );
        await SessionStorageService.SetItemAsStringAsync("CSRF:State", csrf, cancellationToken);

        var request = new OauthLoginRequest(CloudedOptions.Function.Repositories.Github.ClientId)
        {
            Scopes = { "repo", "user" },
            RedirectUri = new Uri(
                Url.Combine(
                    CloudedOptions.Platform.ServerUrl,
                    $"/projects/{ProjectId}/functions/create"
                )
            ),
            State = csrf
        };

        var oauthLoginUrl = _gitHubClient.Oauth.GetGitHubLoginUrl(request);
        navigationManager.NavigateTo(oauthLoginUrl.OriginalString);
    }

    public override async Task<(
        List<FunctionProviderExecutableInput>,
        string?,
        FunctionProject?
    )> LoadFunctionsFromRepositoryBranch(Repository repository, string branchName)
    {
        byte[]? configBytes;
        List<FunctionProviderExecutableInput> projectFunctions = new();
        string? projectConfigContent;
        FunctionProject? projectConfig;

        try
        {
            configBytes = await _gitHubClient.Repository.Content.GetRawContentByRef(
                repository.Owner.Login,
                repository.Name,
                "clouded.yml",
                branchName
            );

            if (configBytes == null)
            {
                throw new FunctionConfigNotFoundException();
            }
        }
        catch
        {
            throw new GettingFunctionsConfigException();
        }

        try
        {
            projectConfigContent = Encoding.UTF8.GetString(configBytes);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            projectConfig = deserializer.Deserialize<FunctionProject>(projectConfigContent);

            foreach (var executable in projectConfig.Executables)
            {
                var annotations = await ScanForAnnotations(
                    repository.Id,
                    executable.Build.Context,
                    branchName
                );

                projectFunctions.AddRange(
                    annotations.Select(
                        x =>
                            new FunctionProviderExecutableInput
                            {
                                ExecutableName = executable.Name,
                                FunctionName = x.FunctionName,
                                FunctionType = x.FunctionType
                            }
                    )
                );
            }
        }
        catch
        {
            throw new FunctionConfigInvalidException();
        }

        return (projectFunctions, projectConfigContent, projectConfig);
    }

    public override async Task<
        IEnumerable<(string FunctionName, EFunctionType FunctionType)>
    > ScanForAnnotations(long repositoryId, string path, string reference)
    {
        var files = await _gitHubClient.Repository.Content.GetAllContentsByRef(
            repositoryId,
            path,
            reference
        );

        var annotations = new List<(string FunctionName, EFunctionType FunctionType)>();

        foreach (var file in files)
        {
            switch (file.Type.Value)
            {
                case ContentType.Dir:
                    annotations.AddRange(
                        await ScanForAnnotations(repositoryId, file.Path, reference)
                    );
                    break;
                case ContentType.File:
                    if (Path.GetExtension(file.Name) == ".cs")
                    {
                        var contentResponse = await _gitHubClient.Connection.GetRawStream(
                            new Uri(file.DownloadUrl),
                            null
                        );

                        annotations.AddRange(
                            FindAnnotations(contentResponse.HttpResponse.Body.ToString())
                        );
                    }

                    break;
            }
        }

        return annotations;
    }

    public override async Task<IReadOnlyList<Repository>> GetRepositories()
    {
        return await _gitHubClient.Repository.GetAllForCurrent();
    }

    public override async Task<IReadOnlyList<Branch>> GetBranches(long repositoryId)
    {
        var githubRepository = await _gitHubClient.Repository.Get(repositoryId);

        return await _gitHubClient.Repository.Branch.GetAll(githubRepository.Id);
    }

    private async Task Validate(string code, string state, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            return;

        var expectedState = await SessionStorageService.GetItemAsStringAsync(
            "CSRF:State",
            cancellationToken
        );
        if (state != expectedState)
            throw new InvalidOperationException("This sorcery is not allowed!");

        await SessionStorageService.RemoveItemAsync("RepositoryType", cancellationToken);
        await SessionStorageService.RemoveItemAsync("CSRF:State", cancellationToken);

        var request = new OauthTokenRequest(
            CloudedOptions.Function.Repositories.Github.ClientId,
            CloudedOptions.Function.Repositories.Github.ClientSecret,
            code
        );
        var oauthToken = await _gitHubClient.Oauth.CreateAccessToken(request);
        _gitHubClient.Credentials = new Credentials(oauthToken.AccessToken);

        if (CurrentUserIntegration == null)
        {
            await Context.CreateAsync(
                new UserIntegrationEntity { GithubOauthToken = oauthToken.AccessToken },
                cancellationToken
            );
        }
        else
        {
            await Context.UpdateAsync<UserIntegrationEntity>(
                CurrentUserIntegration.Id,
                entity =>
                {
                    entity.GithubOauthToken = oauthToken.AccessToken;
                },
                cancellationToken
            );
        }

        await Context.SaveChangesAsync(cancellationToken);

        CurrentUserIntegration = (
            await UserIntegrationService.CurrentIntegrationAsync(
                cancellationToken: cancellationToken
            )
        )!;

        _gitHubClient = new(new ProductHeaderValue("Clouded"))
        {
            Credentials = new Credentials(CurrentUserIntegration.GithubOauthToken)
        };
    }
}
