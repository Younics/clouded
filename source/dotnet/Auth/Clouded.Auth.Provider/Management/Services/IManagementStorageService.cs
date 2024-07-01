namespace Clouded.Auth.Provider.Management.Services;

public interface IManagementStorageService
{
    public Task<string?> GetIdentity();
    public Task SaveIdentity(string identity);
    public Task DeleteIdentity();

    public Task<string?> GetPassword();
    public Task SavePassword(string password);
    public Task DeletePassword();

    public Task<string?> GetToken();
    public Task SaveToken(string token);
    public Task DeleteToken();
}
