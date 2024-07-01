using AutoMapper;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models;
using Clouded.Platform.Models.Dtos.Provider.Admin;
using Clouded.Platform.Shared;
using Clouded.Shared;
using Clouded.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using WebSupport.Library;
using WebSupport.Library.Dtos;

namespace Clouded.Platform.App.Web.Services;

public class AdminProviderService : ProviderService<AdminProviderEntity>, IAdminProviderService
{
    private readonly string _domain;
    private readonly ProviderOptions _providerOptions;
    private readonly IDbContextResolver _dbContextResolver;
    private readonly WebSupportClient _webSupportClient;
    private readonly NavigationManager _navigationManager;
    private readonly IMapper _mapper;

    public AdminProviderService(
        ApplicationOptions options,
        IDbContextResolver dbContextResolver,
        NavigationManager navigationManager,
        IMapper mapper,
        IWebHostEnvironment environment
    )
        : base(options, environment)
    {
        _domain = options.Clouded.Domain;
        _providerOptions = options.Clouded.Provider;
        _dbContextResolver = dbContextResolver;
        _navigationManager = navigationManager;
        _mapper = mapper;

        var webSupportOptions = options.WebSupport;
        _webSupportClient = new WebSupportClient(
            webSupportOptions.ApiKey,
            webSupportOptions.Secret
        );
    }

    public async Task<AdminProviderEntity?> CreateAsync(
        AdminProviderInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var adminProvider = new AdminProviderEntity
        {
            Name = input.Name!,
            Code = input.CodePrefix! + input.Code!,
            DataSourcesRelation = input.DataSourceProviderIds
                .Select(id => new AdminProviderDataSourceRelationEntity { DataSourceId = id })
                .ToList(),
            Configuration = new AdminConfigurationEntity
            {
                PasswordKey = Generator
                    .RandomString(64)
                    .Encrypt(DatabaseOptions.CloudedConnection.EncryptionKey),
                IdentityKey = Generator
                    .RandomString(64)
                    .Encrypt(DatabaseOptions.CloudedConnection.EncryptionKey),
                TokenKey = Generator
                    .RandomString(64)
                    .Encrypt(DatabaseOptions.CloudedConnection.EncryptionKey)
            },
            ProjectId = input.ProjectId!.Value,
        };

        await context.CreateAsync(adminProvider, cancellationToken);

        if (Env.IsProduction)
        {
            var dnsRecord = await _webSupportClient.Dns.RecordCreate(
                _domain,
                new RecordCreateInput
                {
                    Type = "A",
                    Name = adminProvider.Code,
                    Content = _providerOptions.HostIpAddress,
                },
                cancellationToken
            );

            if (dnsRecord?.Status == "success")
            {
                adminProvider.DomainRecordId = (int?)dnsRecord.Item?.Id;
            }
            else
            {
                // Handle error
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return adminProvider;
    }

    public async Task<AdminProviderEntity> UpdateAsync(
        AdminProviderInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        input.Tables = input.Tables.Where(i => i.Enabled).ToList();

        var adminProvider = await context.UpdateAsync<AdminProviderEntity>(
            input.Id!.Value,
            entity =>
            {
                var updatedEntity = _mapper.Map(input, entity);
                updatedEntity.UserAccess = updatedEntity.UserAccess
                    .Select(usr =>
                    {
                        usr.Password = usr.Password.Encrypt(
                            DatabaseOptions.CloudedConnection.EncryptionKey
                        );
                        return usr;
                    })
                    .ToList();

                List<AdminProviderFunctionRelationEntity> assignedFunctions = new();
                GatherAllFunctionInBlock(
                    assignedFunctions,
                    input.CreateFunctions,
                    EAdminProviderFunctionTrigger.Create
                );
                GatherAllFunctionInBlock(
                    assignedFunctions,
                    input.UpdateFunctions,
                    EAdminProviderFunctionTrigger.Update
                );
                GatherAllFunctionInBlock(
                    assignedFunctions,
                    input.DeleteFunctions,
                    EAdminProviderFunctionTrigger.Delete
                );

                updatedEntity.FunctionsRelation = assignedFunctions.ToList();

                foreach (var adminTablesConfigurationEntity in updatedEntity.Tables)
                {
                    List<AdminProviderTableFunctionRelationEntity> assignedTableFunctions = new();
                    GatherAllFunctionInBlockForTable(
                        assignedTableFunctions,
                        input.Tables
                            .First(i => i.Id == adminTablesConfigurationEntity.Id)
                            .CreateFunctions,
                        EAdminProviderFunctionTrigger.Create
                    );
                    GatherAllFunctionInBlockForTable(
                        assignedTableFunctions,
                        input.Tables
                            .First(i => i.Id == adminTablesConfigurationEntity.Id)
                            .UpdateFunctions,
                        EAdminProviderFunctionTrigger.Update
                    );
                    GatherAllFunctionInBlockForTable(
                        assignedTableFunctions,
                        input.Tables
                            .First(i => i.Id == adminTablesConfigurationEntity.Id)
                            .DeleteFunctions,
                        EAdminProviderFunctionTrigger.Delete
                    );

                    adminTablesConfigurationEntity.FunctionsRelation = assignedTableFunctions;
                }
            },
            cancellationToken
        );

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return adminProvider;

        void GatherAllFunctionInBlock(
            List<AdminProviderFunctionRelationEntity> assignedFunctions,
            AdminProviderFunctionsBlockInput blockOfFunctions,
            EAdminProviderFunctionTrigger trigger
        )
        {
            GatherAllFunctionRelations(
                assignedFunctions,
                blockOfFunctions.ValidationHooks,
                trigger
            );
            GatherAllFunctionRelations(assignedFunctions, blockOfFunctions.BeforeHooks, trigger);
            GatherAllFunctionRelations(assignedFunctions, blockOfFunctions.InputHooks, trigger);
            GatherAllFunctionRelations(assignedFunctions, blockOfFunctions.AfterHooks, trigger);
        }

        void GatherAllFunctionRelations(
            List<AdminProviderFunctionRelationEntity> assignedFunctions,
            IEnumerable<long> hooks,
            EAdminProviderFunctionTrigger trigger
        )
        {
            assignedFunctions.AddRange(
                hooks
                    .Select(
                        x =>
                            new AdminProviderFunctionRelationEntity
                            {
                                FunctionId = x,
                                OperationType = trigger,
                            }
                    )
                    .ToList()
            );
        }

        void GatherAllFunctionInBlockForTable(
            List<AdminProviderTableFunctionRelationEntity> assignedFunctions,
            AdminProviderFunctionsBlockInput blockOfFunctions,
            EAdminProviderFunctionTrigger trigger
        )
        {
            GatherAllFunctionRelationsForTable(
                assignedFunctions,
                blockOfFunctions.ValidationHooks,
                trigger
            );
            GatherAllFunctionRelationsForTable(
                assignedFunctions,
                blockOfFunctions.BeforeHooks,
                trigger
            );
            GatherAllFunctionRelationsForTable(
                assignedFunctions,
                blockOfFunctions.InputHooks,
                trigger
            );
            GatherAllFunctionRelationsForTable(
                assignedFunctions,
                blockOfFunctions.AfterHooks,
                trigger
            );
        }

        void GatherAllFunctionRelationsForTable(
            List<AdminProviderTableFunctionRelationEntity> assignedFunctions,
            IEnumerable<long> hooks,
            EAdminProviderFunctionTrigger trigger
        )
        {
            assignedFunctions.AddRange(
                hooks
                    .Select(
                        x =>
                            new AdminProviderTableFunctionRelationEntity
                            {
                                FunctionId = x,
                                OperationType = trigger,
                            }
                    )
                    .ToList()
            );
        }
    }

    public async Task DeleteAsync(
        AdminProviderEntity provider,
        HubConnection providerHubConnection,
        ISnackbar snackbar,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        if (provider.DomainRecordId.HasValue)
        {
            var dnsRecord = await _webSupportClient.Dns.RecordDelete(
                _domain,
                provider.DomainRecordId.Value,
                cancellationToken
            );

            if (dnsRecord?.Status != "success")
            {
                // TODO: Handle error
                throw new ApplicationException();
            }
        }

        await Stop(provider, providerHubConnection, snackbar, cancellationToken);
        context.Delete(provider);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<FileStream> SelfDeploy(AdminProviderEntity authProvider)
    {
        var adminEnvs = ProviderEnvHelper.ComposeAdminEnvs(
            authProvider,
            DatabaseOptions.CloudedConnection.EncryptionKey
        );
        var providerRepository = _providerOptions.Docker.AdminProviderImage;

        return await GetSelfDeployZip(adminEnvs, providerRepository);
    }

    public String GetEditRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/admin_panels/{modelId}/edit";
    }

    public String GetDetailRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/admin_panels/{modelId}";
    }

    public String GetListRoute(string projectId)
    {
        return $"/projects/{projectId}/admin_panels";
    }
}
