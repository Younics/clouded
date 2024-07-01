using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IAuthService : IBaseService
{
    public Task<bool> IsAuthenticatedAsync();
    public Task<long?> CurrentAuthIdAsync();

    public Task<UserEntity?> CurrentUserAsync(
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task<bool> LoginAsync(
        string identity,
        string password,
        CancellationToken cancellationToken = default
    );

    public Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);

    public Task LogoutAsync();

    public Task<UserEntity?> RegisterAsync(
        RegisterInput userEntity,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task<UserIntegrationEntity> RegisterHarborUserAsync(
        UserEntity user,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );
}
