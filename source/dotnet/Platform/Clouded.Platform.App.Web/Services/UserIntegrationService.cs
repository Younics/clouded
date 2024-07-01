using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services;

public class UserIntegrationService(IDbContextResolver dbContextResolver, IAuthService authService)
    : IUserIntegrationService
{
    public async Task<UserIntegrationEntity?> CurrentIntegrationAsync(
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();

        var currentUser = await authService.CurrentUserAsync(cancellationToken: cancellationToken);

        if (currentUser == null)
            return null;

        var userIntegration =
            await context.GetAsync<UserIntegrationEntity>(
                entity => entity.UserId == currentUser.Id,
                cancellationToken: cancellationToken
            ) ?? await authService.RegisterHarborUserAsync(currentUser, context, cancellationToken);

        return userIntegration;
    }
}
