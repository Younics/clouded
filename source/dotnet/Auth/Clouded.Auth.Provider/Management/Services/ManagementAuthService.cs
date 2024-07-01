using Clouded.Auth.Provider.Options;
using Clouded.Shared.Cryptography;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Management.Services;

public class ManagementAuthService(
    ApplicationOptions options,
    IManagementStorageService storageService
) : IManagementAuthService
{
    private readonly AuthManagementOptions _options = options.Clouded.Auth.Management!;
    private readonly CryptoHmacSha512 _cryptoHmac = new();

    public async Task<bool> IsAuthenticatedAsync()
    {
        var identityHash = await storageService.GetIdentity();
        var passwordHash = await storageService.GetPassword();
        var token = await storageService.GetToken();

        return token == Token(identityHash, passwordHash);
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

        await storageService.SaveIdentity(identityHash);
        await storageService.SavePassword(passwordHash);
        await storageService.SaveToken(Token(identityHash, passwordHash));

        return true;
    }

    public async Task LogoutAsync()
    {
        await storageService.DeleteIdentity();
        await storageService.DeletePassword();
        await storageService.DeleteToken();
    }

    private string Token(string? identity, string? password) =>
        _cryptoHmac.Hash($"{identity}:{password}", _options.TokenKey);

    private string IdentityHash(string identity) =>
        _cryptoHmac.Hash(identity, _options.IdentityKey);

    private string PasswordHash(string password) =>
        _cryptoHmac.Hash(password, _options.PasswordKey);
}
