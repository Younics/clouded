using System.Text.RegularExpressions;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Hub.Options;
using Clouded.Platform.Hub.Security;
using Clouded.Platform.Models.Dtos.Hub;
using Clouded.Platform.Models.Dtos.Provider;
using Clouded.Platform.Models.Enums;
using Clouded.Platform.Shared;
using Clouded.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Octokit;
using RepositoryType = Clouded.Shared.Enums.RepositoryType;

namespace Clouded.Platform.Hub.Provider;

public class ProviderHub(ApplicationOptions options, IDbContextResolver dbContextResolver)
    : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly string _domain = options.Clouded.Domain;
    private readonly ProviderOptions _providerOptions = options.Clouded.Provider;
    private readonly HarborOptions _harborOptions = options.Clouded.Harbor;
    private readonly ERegionCode _regionCode = options.Clouded.RegionCode;
    private readonly DatabaseOptions _database = options.Clouded.Database;

    [Authorize]
    public async Task Create(long id, EProviderType type)
    {
        if (Context.UserIdentifier == null)
            return;

        var deployAt = DateTime.UtcNow;
        var context = dbContextResolver.UseScopedContext<CloudedDbContext>();
        var clientProxy = Clients.User(Context.UserIdentifier);
        var user = await context.GetAsync<UserEntity>(
            entity => entity.Id == long.Parse(Context.UserIdentifier),
            Context.ConnectionAborted
        );

        // TODO: Validate if provider id belongs to project that user have access to
        if (user == null)
            return;

        var output = new HubProviderProcessingOutput
        {
            Id = id,
            Success = false,
            Type = type,
            DeployedAt = deployAt,
            SuccessMessage = $"{type} provider deployed successfully",
            ErrorMessage = $"There was an error deploying {type} provider"
        };

        ProviderEntity? provider = null;
        var providerEnvs = new Dictionary<string, string>();

        switch (type)
        {
            case EProviderType.Auth:
            {
                var authProvider = await context.GetAsync<AuthProviderEntity>(
                    id,
                    Context.ConnectionAborted
                );

                if (authProvider != null)
                {
                    providerEnvs = ProviderEnvHelper.ComposeAuthEnvs(
                        authProvider,
                        _domain,
                        _database.CloudedConnection.EncryptionKey
                    );
                    provider = authProvider;

                    await context.UpdateAsync<AuthProviderEntity>(
                        id,
                        p =>
                        {
                            p.DeployedAt = deployAt;
                        }
                    );
                }

                break;
            }
            case EProviderType.Admin:
                var adminProvider = await context.GetAsync<AdminProviderEntity>(
                    id,
                    Context.ConnectionAborted
                );
                if (adminProvider != null)
                {
                    providerEnvs = ProviderEnvHelper.ComposeAdminEnvs(
                        adminProvider,
                        _database.CloudedConnection.EncryptionKey
                    );
                    provider = adminProvider;

                    await context.UpdateAsync<AdminProviderEntity>(
                        id,
                        p =>
                        {
                            p.DeployedAt = deployAt;
                        }
                    );
                }

                break;
            case EProviderType.Api:
                break;
            case EProviderType.Function:
                var functionProvider = await context.GetAsync<FunctionProviderEntity>(
                    id,
                    Context.ConnectionAborted
                );
                if (functionProvider != null)
                {
                    providerEnvs = ProviderEnvHelper.ComposeFunctionEnvs(functionProvider);
                    provider = functionProvider;

                    await context.UpdateAsync<FunctionProviderEntity>(
                        id,
                        p =>
                        {
                            p.DeployedAt = deployAt;
                        }
                    );
                }

                break;
        }

        if (provider != null)
        {
            var input = new ProviderCreateInput
            {
                Id = provider.Id,
                Name = provider.Code,
                Type = provider.Type,
                UserId = Context.UserIdentifier,
                Envs = providerEnvs,
                DeployAt = deployAt,
                HubRegionCode = _regionCode
            };

            if (type == EProviderType.Function)
                input.CustomImage =
                    $"{_harborOptions.Server}/{user.Integration.HarborProject}/{provider.Code}:latest";

            output.Success = await ProviderClient.CreateAsync(
                _providerOptions.RegionServerUrls[provider.Project.Region.Code.GetEnumName()],
                input,
                _providerOptions.ApiKey,
                Context.ConnectionAborted
            );
        }

        if (output.Success)
        {
            // save updated deployedAt
            await context.SaveChangesAsync();
        }

        await clientProxy.SendAsync(
            nameof(HubProviderProcessingOutput),
            output,
            Context.ConnectionAborted
        );
    }

    [Authorize]
    public async Task Start(long id, EProviderType type)
    {
        var userId = Context.UserIdentifier;
        if (userId == null)
            return;

        var context = dbContextResolver.UseScopedContext<CloudedDbContext>();
        var clientProxy = Clients.User(userId);

        var output = new HubProviderProcessingOutput
        {
            Id = id,
            Success = false,
            Type = type,
            SuccessMessage = $"{type} provider start process started",
            ErrorMessage = $"There was an error starting {type} provider"
        };

        ProviderEntity? provider = null;
        Dictionary<string, string>? providerEnvs = null;

        switch (type)
        {
            case EProviderType.Auth:
                provider = await context.GetAsync<AuthProviderEntity>(
                    id,
                    Context.ConnectionAborted
                );
                providerEnvs = ProviderEnvHelper.ComposeAuthEnvs(
                    (AuthProviderEntity?)provider,
                    _domain,
                    _database.CloudedConnection.EncryptionKey
                );
                break;
            case EProviderType.Admin:
                provider = await context.GetAsync<AdminProviderEntity>(
                    id,
                    Context.ConnectionAborted
                );
                // providerEnvs = ComposeAdminEnvs((AdminProviderEntity?) provider);
                break;
            // TODO
            case EProviderType.Api:
                // provider = await context.GetAsync<ApiProviderEntity>(id, Context.ConnectionAborted);
                // providerEnvs = ComposeApiEnvs((ApiProviderEntity?) provider);
                break;
            case EProviderType.Function:
                provider = await context.GetAsync<FunctionProviderEntity>(
                    id,
                    Context.ConnectionAborted
                );
                providerEnvs = ProviderEnvHelper.ComposeFunctionEnvs(
                    (FunctionProviderEntity?)provider
                );
                break;
        }
        ;

        if (provider != null)
        {
            output.Success = await ProviderClient.StartAsync(
                _providerOptions.RegionServerUrls[provider.Project.Region.Code.GetEnumName()],
                provider.Code,
                providerEnvs,
                _providerOptions.ApiKey,
                Context.ConnectionAborted
            );
        }

        await clientProxy.SendAsync(
            nameof(HubProviderProcessingOutput),
            output,
            Context.ConnectionAborted
        );
    }

    [Authorize]
    public async Task Stop(long id, EProviderType type)
    {
        if (Context.UserIdentifier == null)
            return;

        var context = dbContextResolver.UseScopedContext<CloudedDbContext>();
        var clientProxy = Clients.User(Context.UserIdentifier);

        var output = new HubProviderProcessingOutput
        {
            Id = id,
            Success = false,
            Type = type,
            SuccessMessage = $"{type} provider stop process started",
            ErrorMessage = $"There was an error stopping {type} provider"
        };

        var provider = type switch
        {
            EProviderType.Auth
                => (await context.GetAsync<AuthProviderEntity>(id, Context.ConnectionAborted))!
                    as ProviderEntity,
            EProviderType.Admin
                => (await context.GetAsync<AdminProviderEntity>(id, Context.ConnectionAborted))!
                    as ProviderEntity,
            // TODO
            // EProviderType.Api => (await context.GetAsync<ApiProviderEntity>(id, Context.ConnectionAborted))! as ProviderEntity,
            EProviderType.Function
                => (await context.GetAsync<FunctionProviderEntity>(id, Context.ConnectionAborted))!
                    as ProviderEntity,
            _ => null
        };

        if (provider != null)
        {
            output.Success = await ProviderClient.StopAsync(
                _providerOptions.RegionServerUrls[provider.Project.Region.Code.GetEnumName()],
                provider.Code,
                _providerOptions.ApiKey,
                Context.ConnectionAborted
            );
        }

        await clientProxy.SendAsync(
            nameof(HubProviderProcessingOutput),
            output,
            Context.ConnectionAborted
        );
    }

    [Authorize]
    public async Task Delete(long id, EProviderType type)
    {
        if (Context.UserIdentifier == null)
            return;

        var context = dbContextResolver.UseScopedContext<CloudedDbContext>();
        var clientProxy = Clients.User(Context.UserIdentifier);

        var output = new HubProviderProcessingOutput
        {
            Id = id,
            Success = false,
            Type = type,
            SuccessMessage = $"{type} provider stop process started",
            ErrorMessage = $"There was an error stopping {type} provider"
        };

        var provider = type switch
        {
            EProviderType.Auth
                => (await context.GetAsync<AuthProviderEntity>(id, Context.ConnectionAborted))!
                    as ProviderEntity,
            EProviderType.Admin
                => (await context.GetAsync<AdminProviderEntity>(id, Context.ConnectionAborted))!
                    as ProviderEntity,
            // TODO
            // EProviderType.Api => (await context.GetAsync<ApiProviderEntity>(id, Context.ConnectionAborted))! as ProviderEntity,
            EProviderType.Function
                => (await context.GetAsync<FunctionProviderEntity>(id, Context.ConnectionAborted))!
                    as ProviderEntity,
            _ => null
        };

        if (provider != null)
        {
            output.Success = await ProviderClient.DeleteAsync(
                _providerOptions.RegionServerUrls[provider.Project.Region.Code.GetEnumName()],
                provider.Code,
                _providerOptions.ApiKey,
                Context.ConnectionAborted
            );
        }

        await clientProxy.SendAsync(
            nameof(HubProviderProcessingOutput),
            output,
            Context.ConnectionAborted
        );
    }

    [CloudedAuthorize]
    public async Task UpdateStatus(HubProviderUpdateStatus input)
    {
        var context = dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(Context.ConnectionAborted);

        if (input.Status == EProviderStatus.Terminated)
        {
            switch (input.Type)
            {
                case EProviderType.Auth:
                    context.Delete<AuthProviderEntity>(input.Id);
                    break;
                case EProviderType.Admin:
                    context.Delete<AdminProviderEntity>(input.Id);
                    break;
                case EProviderType.Api:
                    // TODO
                    // context.Delete<ApiProviderEntity>(input.Id);
                    break;
                case EProviderType.Function:
                    context.Delete<FunctionProviderEntity>(input.Id);
                    break;
            }
        }
        else
        {
            switch (input.Type)
            {
                case EProviderType.Auth:
                    await context.UpdateAsync<AuthProviderEntity>(
                        input.Id,
                        provider =>
                        {
                            if (
                                input.DeployAt != null
                                && provider.DeployedAt != null
                                && DateTime.Compare(
                                    (DateTime)input.DeployAt,
                                    (DateTime)provider.DeployedAt
                                ) == 0
                            )
                            {
                                provider.Status = input.Status;
                            }
                        }
                    );
                    break;
                case EProviderType.Admin:
                    await context.UpdateAsync<AdminProviderEntity>(
                        input.Id,
                        provider =>
                        {
                            if (
                                input.DeployAt != null
                                && provider.DeployedAt != null
                                && DateTime.Compare(
                                    (DateTime)input.DeployAt,
                                    (DateTime)provider.DeployedAt
                                ) == 0
                            )
                            {
                                provider.Status = input.Status;
                            }
                        }
                    );
                    break;
                case EProviderType.Api:
                    // TODO
                    // await context.UpdateAsync<ApiProviderEntity>
                    // (
                    //     input.Id,
                    //     provider =>
                    //     {
                    //         provider.Status = input.Status;
                    //     }
                    // );
                    break;
                case EProviderType.Function:
                    await context.UpdateAsync<FunctionProviderEntity>(
                        input.Id,
                        provider =>
                        {
                            if (
                                input.DeployAt != null
                                && provider.DeployedAt != null
                                && DateTime.Compare(
                                    (DateTime)input.DeployAt,
                                    (DateTime)provider.DeployedAt
                                ) == 0
                            )
                            {
                                provider.Status = input.Status;
                            }
                        }
                    );
                    break;
            }
        }

        await context.SaveChangesAsync(Context.ConnectionAborted);
        await transaction.CommitAsync(Context.ConnectionAborted);

        if (input.UserId != null)
            await Clients.User(input.UserId).SendAsync(nameof(HubProviderUpdateStatus), input);
    }

    [Authorize]
    public async Task FunctionBuild(long id, string branch)
    {
        if (Context.UserIdentifier == null)
            return;

        var context = dbContextResolver.UseScopedContext<CloudedDbContext>();
        var clientProxy = Clients.User(Context.UserIdentifier);
        var user = await context.GetAsync<UserEntity>(
            entity => entity.Id == long.Parse(Context.UserIdentifier),
            Context.ConnectionAborted
        );

        if (user == null)
            return;

        var output = new HubProviderProcessingOutput { Id = id, Success = false };

        var functionProvider = await context.GetAsync<FunctionProviderEntity>(
            id,
            Context.ConnectionAborted
        );
        if (functionProvider != null)
        {
            // TODO: Refactor to repository core
            var gitHubClient = new GitHubClient(new ProductHeaderValue("Clouded"))
            {
                Credentials = new Credentials(user.Integration.GithubOauthToken)
            };
            var repository = await gitHubClient.Repository.Get(
                long.Parse(functionProvider.Configuration.RepositoryId)
            );
            var urlMatch = Regex.Match(
                repository.CloneUrl,
                @"(?<=\:\/\/)(.*)",
                RegexOptions.Singleline
            );

            output.Success = await ProviderClient.FunctionBuildAsync(
                _providerOptions.RegionServerUrls[
                    functionProvider.Project.Region.Code.GetEnumName()
                ],
                new FunctionBuildInput
                {
                    Id = functionProvider.Id,
                    Name = functionProvider.Code,
                    UserId = Context.UserIdentifier,
                    HubRegionCode = _regionCode,
                    Image =
                        $"{_harborOptions.Server}/{user.Integration.HarborProject}/{functionProvider.Code}",
                    GitRepositoryUrl = urlMatch.Value,
                    GitRepositoryToken = (
                        functionProvider.Configuration.RepositoryType switch
                        {
                            RepositoryType.Github => user.Integration.GithubOauthToken,
                            // TODO: Change to gitlab token
                            RepositoryType.Gitlab
                                => user.Integration.GithubOauthToken,
                            _ => string.Empty
                        }
                    )!,
                    GitRepositoryBranch = branch,
                },
                _providerOptions.ApiKey,
                Context.ConnectionAborted
            );
        }

        // TODO: In future we should've build history/logs with status etc...

        await clientProxy.SendAsync(
            nameof(HubProviderProcessingOutput),
            output,
            Context.ConnectionAborted
        );
    }
}
