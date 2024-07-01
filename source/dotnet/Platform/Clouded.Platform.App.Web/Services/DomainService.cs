using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services;

public class DomainService(
    ApplicationOptions options,
    IDbContextResolver dbContextResolver,
    NavigationManager navigationManager
) : IDomainService
{
    protected readonly DatabaseOptions DatabaseOptions = options.Clouded.Database;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task<DomainEntity?> CreateAsync(
        DomainInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var domain = new DomainEntity { Value = input.Value!, ProjectId = input.ProjectId!.Value, };

        await context.CreateAsync(domain, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return domain;
    }

    public async Task<DomainEntity> UpdateAsync(
        DomainInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var domain = await context.UpdateAsync<DomainEntity>(
            input.Id!.Value,
            entity =>
            {
                entity.Value = input.Value!;
                entity.ProjectId = input.ProjectId!.Value;
            },
            cancellationToken
        );

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return domain;
    }

    public async Task DeleteAsync(
        DomainEntity entity,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        context.Delete(entity);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public String GetEditRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/domains/{modelId}/edit";
    }

    public String GetDetailRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/domains/{modelId}";
    }

    public String GetListRoute(string projectId)
    {
        return $"/projects/{projectId}/domains";
    }
}
