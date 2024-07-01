using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IDomainService : IBaseService
{
    public Task<DomainEntity?> CreateAsync(
        DomainInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task<DomainEntity> UpdateAsync(
        DomainInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task DeleteAsync(
        DomainEntity entity,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public String GetEditRoute(string projectId, long? modelId);
    public String GetDetailRoute(string projectId, long? modelId);
    public String GetListRoute(string projectId);
}
