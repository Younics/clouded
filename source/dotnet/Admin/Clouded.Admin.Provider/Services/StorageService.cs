using System.Security.Claims;
using System.Security.Cryptography;
using Blazored.LocalStorage;
using Clouded.Admin.Provider.Services.Interfaces;

namespace Clouded.Admin.Provider.Services;

public class StorageService(ILocalStorageService localStorage) : IStorageService
{
    private const string StorageIdentityId = "pip";
    private const string StorageIdentity = "pp";
    private const string StoragePassword = "qt";
    private const string StorageToken = "mt";

    public async Task<IEnumerable<Claim>> UserClaims()
    {
        try
        {
            var identity = await GetIdentityId();
            if (identity == null)
            {
                throw new CryptographicException();
            }

            return new List<Claim>
            {
                new(ClaimTypes.Name, identity),
                new(ClaimTypes.NameIdentifier, identity),
                new("blocked", $"{false}")
            };
        }
        catch (CryptographicException)
        {
            return Array.Empty<Claim>();
        }
    }

    public async Task<string?> GetIdentityId() =>
        (await localStorage.GetItemAsync<string>(StorageIdentityId));

    public async Task SaveIdentityId(string identityId) =>
        await localStorage.SetItemAsync(StorageIdentityId, identityId);

    public async Task DeleteIdentityId() => await localStorage.RemoveItemAsync(StorageIdentityId);

    public async Task<string?> GetIdentity() =>
        (await localStorage.GetItemAsync<string>(StorageIdentity));

    public async Task SaveIdentity(string identity) =>
        await localStorage.SetItemAsync(StorageIdentity, identity);

    public async Task DeleteIdentity() => await localStorage.RemoveItemAsync(StorageIdentity);

    public async Task<string?> GetPassword() =>
        (await localStorage.GetItemAsync<string>(StoragePassword));

    public async Task SavePassword(string password) =>
        await localStorage.SetItemAsync(StoragePassword, password);

    public async Task DeletePassword() => await localStorage.RemoveItemAsync(StoragePassword);

    public async Task<string?> GetToken() =>
        (await localStorage.GetItemAsync<string>(StorageToken));

    public async Task SaveToken(string token) =>
        await localStorage.SetItemAsync(StorageToken, token);

    public async Task DeleteToken() => await localStorage.RemoveItemAsync(StorageToken);
}
