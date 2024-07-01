using System.Security.Claims;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IStorageService : IBaseService
{
    public Task<IEnumerable<Claim>> UserClaims();
    public Task<string?> GetAccessToken();
    public Task<string?> GetRefreshToken();
    public Task SaveTokens(string accessToken, string refreshToken);
    public Task DeleteTokens();
}
