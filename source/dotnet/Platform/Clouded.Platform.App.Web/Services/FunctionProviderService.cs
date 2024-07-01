using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models;
using Clouded.Platform.Models.Enums;
using Clouded.Shared;
using Clouded.Shared.Cryptography;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using WebSupport.Library;
using WebSupport.Library.Dtos;

namespace Clouded.Platform.App.Web.Services;

public class FunctionProviderService
    : ProviderService<FunctionProviderEntity>,
        IFunctionProviderService
{
    private readonly string _domain;
    private readonly ProviderOptions _providerOptions;
    private readonly IDbContextResolver _dbContextResolver;
    private readonly WebSupportClient _webSupportClient;
    private readonly NavigationManager _navigationManager;

    public FunctionProviderService(
        ApplicationOptions options,
        IDbContextResolver dbContextResolver,
        NavigationManager navigationManager,
        IWebHostEnvironment environment
    )
        : base(options, environment)
    {
        _domain = options.Clouded.Domain;
        _providerOptions = options.Clouded.Provider;
        _dbContextResolver = dbContextResolver;
        _navigationManager = navigationManager;

        var webSupportOptions = options.WebSupport;
        _webSupportClient = new WebSupportClient(
            webSupportOptions.ApiKey,
            webSupportOptions.Secret
        );
    }

    public async Task<FunctionProviderEntity?> CreateAsync(
        FunctionProviderInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var functionProvider = new FunctionProviderEntity
        {
            Name = input.Name,
            Code = input.CodePrefix + new CryptoAdler32().Hash(input.Name),
            Type = EProviderType.Function,
            Configuration = new FunctionConfigurationEntity
            {
                ApiKey = Generator.RandomString(256),
                RepositoryId = input.RepositoryId,
                RepositoryType = input.RepositoryType
            },
            Functions = input.Hooks
                .Select(
                    hook =>
                        new FunctionEntity
                        {
                            ExecutableName = hook.ExecutableName,
                            Name = hook.FunctionName,
                            Type = hook.FunctionType
                        }
                )
                .ToList(),
            ProjectId = input.ProjectId
        };

        await context.CreateAsync(functionProvider, cancellationToken);

        if (Env.IsProduction)
        {
            var dnsRecord = await _webSupportClient.Dns.RecordCreate(
                _domain,
                new RecordCreateInput
                {
                    Type = "A",
                    Name = functionProvider.Code,
                    Content = _providerOptions.HostIpAddress
                },
                cancellationToken
            );

            if (dnsRecord?.Status == "success")
            {
                functionProvider.DomainRecordId = (int?)dnsRecord.Item?.Id;
            }
            else
            {
                // Handle error
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return functionProvider;
    }

    public async Task<FunctionProviderEntity> UpdateAsync(
        FunctionProviderInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var functionProvider = await context.UpdateAsync<FunctionProviderEntity>(
            input.Id!.Value,
            entity =>
            {
                entity.Configuration.Branch = input.Branch;
                entity.Configuration.RepositoryId = input.RepositoryId;
                entity.Configuration.RepositoryType = input.RepositoryType;
                entity.Functions = input.Hooks
                    .Select(
                        hook =>
                            new FunctionEntity
                            {
                                ExecutableName = hook.ExecutableName,
                                Name = hook.FunctionName,
                                Type = hook.FunctionType
                            }
                    )
                    .ToList();
            },
            cancellationToken
        );

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return functionProvider;
    }

    public async Task DeleteAsync(
        FunctionProviderEntity provider,
        HubConnection? providerHubConnection,
        ISnackbar snackbar,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        context.Delete(provider);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public Task<FileStream> SelfDeploy(FunctionProviderEntity authProvider)
    {
        throw new NotImplementedException();
    }

    public Task Deploy(
        FunctionProviderEntity functionProvider,
        ISnackbar snackbar,
        HubConnection providerHubConnection,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task Start(
        FunctionProviderEntity functionProvider,
        ISnackbar snackbar,
        HubConnection providerHubConnection,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task Stop(
        FunctionProviderEntity functionProvider,
        ISnackbar snackbar,
        HubConnection providerHubConnection,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public String GetEditRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/functions/{modelId}/edit";
    }

    public String GetDetailRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/functions/{modelId}";
    }

    public String GetListRoute(string projectId)
    {
        return $"/projects/{projectId}/functions";
    }
}
