using Clouded.Auth.Client;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IAuthProviderManagementService : IBaseService
{
    public Task<AuthManagementClient?> GetClientAsync(
        long authProviderId,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );
}
