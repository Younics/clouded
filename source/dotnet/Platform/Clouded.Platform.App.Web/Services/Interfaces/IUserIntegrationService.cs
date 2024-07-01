using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IUserIntegrationService : IBaseService
{
    public Task<UserIntegrationEntity?> CurrentIntegrationAsync(
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );
}
