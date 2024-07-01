using Blazored.LocalStorage;

namespace Clouded.Auth.Provider.Management.Services;

public class ManagementStorageService(ILocalStorageService localStorage) : IManagementStorageService
{
    private const string StorageIdentity = "pp";
    private const string StoragePassword = "qt";
    private const string StorageToken = "mt";

    public async Task<string?> GetIdentity() =>
        await localStorage.GetItemAsync<string>(StorageIdentity);

    public async Task SaveIdentity(string identity) =>
        await localStorage.SetItemAsStringAsync(StorageIdentity, identity);

    public async Task DeleteIdentity() => await localStorage.RemoveItemAsync(StorageIdentity);

    public async Task<string?> GetPassword() =>
        await localStorage.GetItemAsync<string>(StoragePassword);

    public async Task SavePassword(string password) =>
        await localStorage.SetItemAsStringAsync(StoragePassword, password);

    public async Task DeletePassword() => await localStorage.RemoveItemAsync(StoragePassword);

    public async Task<string?> GetToken() => await localStorage.GetItemAsync<string>(StorageToken);

    public async Task SaveToken(string token) =>
        await localStorage.SetItemAsStringAsync(StorageToken, token);

    public async Task DeleteToken() => await localStorage.RemoveItemAsync(StorageToken);
}
