using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Blazored.LocalStorage;
using Clouded.Platform.App.Web.Services.Interfaces;

namespace Clouded.Platform.App.Web.Services;

public class StorageService(ILocalStorageService localStorage) : IStorageService
{
    private const string StorageAccessToken = "AccessToken";
    private const string StorageRefreshToken = "RefreshToken";

    public async Task<IEnumerable<Claim>> UserClaims()
    {
        try
        {
            var token = await localStorage.GetItemAsync<string>(StorageAccessToken);

            if (string.IsNullOrEmpty(token))
                return Array.Empty<Claim>();

            return new JwtSecurityTokenHandler().ReadToken(token) is JwtSecurityToken securityToken
                ? securityToken.Claims
                : Array.Empty<Claim>();
        }
        catch (CryptographicException)
        {
            return Array.Empty<Claim>();
        }
    }

    public async Task<string?> GetAccessToken()
    {
        return await localStorage.GetItemAsync<string>(StorageAccessToken);
    }

    public async Task<string?> GetRefreshToken()
    {
        return await localStorage.GetItemAsync<string>(StorageRefreshToken);
    }

    public async Task SaveTokens(string accessToken, string refreshToken)
    {
        await localStorage.SetItemAsync(StorageAccessToken, accessToken);
        await localStorage.SetItemAsync(StorageRefreshToken, refreshToken);
    }

    public async Task DeleteTokens()
    {
        await localStorage.RemoveItemAsync(StorageAccessToken);
        await localStorage.RemoveItemAsync(StorageRefreshToken);
    }
}
