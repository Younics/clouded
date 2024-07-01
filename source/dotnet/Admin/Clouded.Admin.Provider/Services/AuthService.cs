using Clouded.Admin.Provider.Options;
using Clouded.Admin.Provider.Services.Interfaces;
using Clouded.Shared.Cryptography;

namespace Clouded.Admin.Provider.Services;

public class AuthService(ApplicationOptions options, IStorageService storageService) : IAuthService
{
    private readonly AuthOptions _options = options.Clouded.Admin.Auth;
    private readonly CryptoHmacSha512 _cryptoHmac = new();

    public async Task<string?> GetUserId()
    {
        return await storageService.GetIdentityId();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var identityId = await storageService.GetIdentityId();
        var identityHash = await storageService.GetIdentity();
        var passwordHash = await storageService.GetPassword();
        var token = await storageService.GetToken();

        return token == Token(identityId, identityHash, passwordHash);
    }

    public async Task<bool> LoginAsync(
        string identity,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        if (!_options.Users.Any(x => x.Identity == identity && x.Password == password))
            return false;

        var identityHash = IdentityHash(identity);
        var passwordHash = PasswordHash(password);
        var id = _options.Users.First(x => x.Identity == identity && x.Password == password).Id;

        await storageService.SaveIdentityId(id);
        await storageService.SaveIdentity(identityHash);
        await storageService.SavePassword(passwordHash);
        await storageService.SaveToken(Token(id, identityHash, passwordHash));

        return true;
    }

    public async Task LogoutAsync()
    {
        await storageService.DeleteIdentityId();
        await storageService.DeleteIdentity();
        await storageService.DeletePassword();
        await storageService.DeleteToken();
    }

    private string Token(string? id, string? identity, string? password) =>
        _cryptoHmac.Hash(BuildStringHash(id, identity, password), _options.TokenKey);

    private string BuildStringHash(string? id, string? identity, string? password) =>
        $"{id}:{identity}:{password}";

    private string IdentityHash(string identity) =>
        _cryptoHmac.Hash(identity, _options.IdentityKey);

    private string PasswordHash(string password) =>
        _cryptoHmac.Hash(password, _options.PasswordKey);
}
