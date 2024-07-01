namespace Clouded.Admin.Provider.Services.Interfaces;

public interface IAuthService
{
    public Task<string?> GetUserId();

    public Task<bool> IsAuthenticatedAsync();

    public Task<bool> LoginAsync(
        string identity,
        string password,
        CancellationToken cancellationToken = default
    );

    public Task LogoutAsync();
}
