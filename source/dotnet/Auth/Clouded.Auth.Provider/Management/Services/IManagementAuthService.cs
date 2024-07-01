namespace Clouded.Auth.Provider.Management.Services;

public interface IManagementAuthService
{
    public Task<bool> IsAuthenticatedAsync();

    public Task<bool> LoginAsync(
        string identity,
        string password,
        CancellationToken cancellationToken = default
    );

    public Task LogoutAsync();
}
