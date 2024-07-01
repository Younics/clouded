using AutoMapper;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models;
using Clouded.Platform.Models.Enums;
using Clouded.Platform.Shared;
using Clouded.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using WebSupport.Library;
using WebSupport.Library.Dtos;

namespace Clouded.Platform.App.Web.Services;

public class AuthProviderService : ProviderService<AuthProviderEntity>, IAuthProviderService
{
    private readonly string _domain;
    private readonly ProviderOptions _providerOptions;
    private readonly IDbContextResolver _dbContextResolver;
    private readonly WebSupportClient _webSupportClient;
    private readonly IMapper _mapper;
    private readonly NavigationManager _navigationManager;
    private readonly DatabaseOptions _databaseOptions;

    public AuthProviderService(
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
        _databaseOptions = options.Clouded.Database;
        _dbContextResolver = dbContextResolver;
        _navigationManager = navigationManager;
        _mapper = mapper;

        var webSupportOptions = options.WebSupport;
        _webSupportClient = new WebSupportClient(
            webSupportOptions.ApiKey,
            webSupportOptions.Secret
        );
    }

    public async Task<AuthProviderEntity?> CreateAsync(
        AuthProviderInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var authProvider = PrepareBeforeSave(input, null);

        await context.CreateAsync(authProvider, cancellationToken);

        if (Env.IsProduction)
        {
            var dnsRecord = await _webSupportClient.Dns.RecordCreate(
                _domain,
                new RecordCreateInput
                {
                    Type = "A",
                    Name = authProvider.Code,
                    Content = _providerOptions.HostIpAddress,
                },
                cancellationToken
            );

            if (dnsRecord?.Status == "success")
            {
                authProvider.DomainRecordId = (int?)dnsRecord.Item?.Id;
            }
            else
            {
                // Handle error
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return authProvider;
    }

    private AuthProviderEntity PrepareBeforeSave(
        AuthProviderInput input,
        AuthProviderEntity? authProviderEntity
    )
    {
        var authProvider =
            authProviderEntity == null
                ? _mapper.Map<AuthProviderEntity>(input)
                : _mapper.Map(input, authProviderEntity);

        foreach (var authUserAccessEntity in authProvider.Configuration.UserAccess)
        {
            authUserAccessEntity.Password = authUserAccessEntity.Password.Encrypt(
                _databaseOptions.CloudedConnection.EncryptionKey
            );
        }

        if (authProviderEntity.Id == null)
        {
            authProvider.Configuration.ApiKey = Generator
                .RandomString(256)
                .Encrypt(_databaseOptions.CloudedConnection.EncryptionKey);
            authProvider.Configuration.Token.Secret = Generator
                .RandomString(128)
                .Encrypt(_databaseOptions.CloudedConnection.EncryptionKey);
            authProvider.Configuration.ManagementIdentityKey = Generator
                .RandomString(256)
                .Encrypt(_databaseOptions.CloudedConnection.EncryptionKey);
            authProvider.Configuration.ManagementPasswordKey = Generator
                .RandomString(256)
                .Encrypt(_databaseOptions.CloudedConnection.EncryptionKey);
            authProvider.Configuration.ManagementTokenKey = Generator
                .RandomString(256)
                .Encrypt(_databaseOptions.CloudedConnection.EncryptionKey);
        }

        if (input.Configuration.Mail is { Enabled: true })
        {
            authProvider.Configuration.Mail!.Host = input.Configuration.Mail.Host!.Encrypt(
                _databaseOptions.CloudedConnection.EncryptionKey
            );

            if (input.Configuration.Mail is { Authentication: true })
            {
                authProvider.Configuration.Mail!.Password =
                    input.Configuration.Mail.Password!.Encrypt(
                        _databaseOptions.CloudedConnection.EncryptionKey
                    );
                authProvider.Configuration.Mail!.User = input.Configuration.Mail.User!.Encrypt(
                    _databaseOptions.CloudedConnection.EncryptionKey
                );
            }
        }

        switch (input.Configuration.Hash.AlgorithmType)
        {
            case EHashType.Argon2:
                authProvider.Configuration.Hash = new AuthHashArgon2ConfigurationEntity
                {
                    DegreeOfParallelism = input.Configuration.ArgonInput.DegreeOfParallelism,
                    MemorySize = input.Configuration.ArgonInput.MemorySize,
                    Iterations = input.Configuration.ArgonInput.Iterations,
                    ReturnBytes = input.Configuration.ArgonInput.ReturnBytes,
                    AlgorithmType = EHashType.Argon2,
                    Secret = input.Configuration.Hash.Secret!.Encrypt(
                        _databaseOptions.CloudedConnection.EncryptionKey
                    )
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var socials = new List<AuthSocialConfigurationEntity>();
        AddSocial(input.Configuration.Facebook, ESocialAuthType.Facebook, socials);
        AddSocial(input.Configuration.Google, ESocialAuthType.Google, socials);
        AddSocial(input.Configuration.Apple, ESocialAuthType.Apple, socials);

        authProvider.Configuration.SocialConfiguration = socials;

        return authProvider;
    }

    private static void AddSocial(
        AuthProviderSocialInput input,
        ESocialAuthType type,
        ICollection<AuthSocialConfigurationEntity> socials
    )
    {
        if (input.Enabled)
        {
            socials.Add(
                new AuthSocialConfigurationEntity
                {
                    Type = type,
                    Secret = input.Secret!,
                    Key = input.Key!,
                    RedirectUrl = input.RedirectUrl!,
                    DeniedRedirectUrl = input.DeniedRedirectUrl!
                }
            );
        }
    }

    public async Task<AuthProviderEntity> UpdateAsync(
        AuthProviderInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= _dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var authProviderEntity = await context.UpdateAsync<AuthProviderEntity>(
            input.Id!.Value,
            entity =>
            {
                PrepareBeforeSave(input, entity);
            },
            cancellationToken
        );

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return authProviderEntity;
    }

    public async Task DeleteAsync(
        AuthProviderEntity provider,
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

    public async Task<FileStream> SelfDeploy(AuthProviderEntity authProvider)
    {
        var authEnvs = ProviderEnvHelper.ComposeAuthEnvs(
            authProvider,
            _domain,
            DatabaseOptions.CloudedConnection.EncryptionKey
        );
        var providerRepository = _providerOptions.Docker.AuthProviderImage;

        return await GetSelfDeployZip(authEnvs, providerRepository);
    }

    public void PrepareInput(AuthProviderEntity entity, AuthProviderInput input)
    {
        input.Configuration.IdentityOrganization.Enabled =
            input.Configuration.IdentityOrganization.Id != null
            && input.Configuration.IdentityOrganization.Id != 0;
        input.Configuration.IdentityPermission.Enabled =
            input.Configuration.IdentityPermission.Id != null
            && input.Configuration.IdentityPermission.Id != 0;
        input.Configuration.IdentityRole.Enabled =
            input.Configuration.IdentityRole.Id != null && input.Configuration.IdentityRole.Id != 0;
        input.Configuration.Google.Enabled =
            input.Configuration.Google.Id != null && input.Configuration.Google.Id != 0;
        input.Configuration.Facebook.Enabled =
            input.Configuration.Facebook.Id != null && input.Configuration.Facebook.Id != 0;
        input.Configuration.Apple.Enabled =
            input.Configuration.Apple.Id != null && input.Configuration.Apple.Id != 0;
        input.Configuration.Mail.Enabled =
            input.Configuration.Mail.Id != null && input.Configuration.Mail.Id != 0;

        input.Configuration.Management.Enabled = entity.Configuration.Management;
        input.Configuration.Management.Users = entity.Configuration.UserAccess
            .Select(
                usr =>
                    new AuthProviderManagementUserInput
                    {
                        Id = usr.Id,
                        Identity = usr.Identity,
                        Password = usr.Password.Decrypt(
                            _databaseOptions.CloudedConnection.EncryptionKey
                        ),
                    }
            )
            .ToList();

        input.Configuration.Hash = new AuthProviderHashInput
        {
            AlgorithmType = entity.Configuration.Hash.AlgorithmType,
            Secret = entity.Configuration.Hash.Secret.Decrypt(
                _databaseOptions.CloudedConnection.EncryptionKey
            )
        };

        if (entity.Configuration.Hash.AlgorithmType == EHashType.Argon2)
        {
            input.Configuration.ArgonInput = new AuthProviderArgonHashInput
            {
                Iterations = (
                    (AuthHashArgon2ConfigurationEntity)entity.Configuration.Hash
                ).Iterations,
                DegreeOfParallelism = (
                    (AuthHashArgon2ConfigurationEntity)entity.Configuration.Hash
                ).DegreeOfParallelism,
                MemorySize = (
                    (AuthHashArgon2ConfigurationEntity)entity.Configuration.Hash
                ).MemorySize,
                ReturnBytes = (
                    (AuthHashArgon2ConfigurationEntity)entity.Configuration.Hash
                ).ReturnBytes,
            };
        }

        if (entity.Configuration.Mail != null)
        {
            input.Configuration.Mail.User = entity.Configuration.Mail.User?.Decrypt(
                _databaseOptions.CloudedConnection.EncryptionKey
            );
            input.Configuration.Mail.Password = entity.Configuration.Mail.Password?.Decrypt(
                _databaseOptions.CloudedConnection.EncryptionKey
            );
            input.Configuration.Mail.Host = entity.Configuration.Mail.Host.Decrypt(
                _databaseOptions.CloudedConnection.EncryptionKey
            );
        }
    }

    public String GetEditRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/authentications/{modelId}/edit";
    }

    public String GetDetailRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/authentications/{modelId}";
    }

    public String GetListRoute(string projectId)
    {
        return $"/projects/{projectId}/authentications";
    }
}
