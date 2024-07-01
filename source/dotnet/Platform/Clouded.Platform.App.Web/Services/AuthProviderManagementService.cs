using Clouded.Auth.Client;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services;

public class AuthProviderManagementService(
    ApplicationOptions options,
    IDbContextResolver dbContextResolver
) : IAuthProviderManagementService
{
    private readonly string _domain = options.Clouded.Domain;
    private readonly ProviderOptions _providerOptions = options.Clouded.Provider;

    public async Task<AuthManagementClient?> GetClientAsync(
        long authProviderId,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var authProvider = await context.GetAsync<AuthProviderEntity>(
            authProviderId,
            cancellationToken
        );

        if (authProvider == null)
            return null;

        await context.LazyLoadAsync(authProvider, x => x.Configuration, cancellationToken);

        return new AuthCloudedClient(
            $"https://{authProvider.Code}.{_domain}",
            cloudedKey: authProvider.Configuration.ApiKey,
            Env.IsDevelopment
        ).Management;
    }
}
