using System.Security.Claims;

namespace Clouded.Admin.Provider.Services.Interfaces;

public interface IStorageService
{
    public Task<IEnumerable<Claim>> UserClaims();

    public Task<string?> GetIdentityId();
    public Task SaveIdentityId(string identityId);
    public Task DeleteIdentityId();

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
