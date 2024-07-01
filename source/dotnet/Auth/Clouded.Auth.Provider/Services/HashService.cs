using Clouded.Auth.Provider.Cryptography;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Shared.Cryptography;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Services;

public class HashService : IHashService
{
    private readonly ICrypto _crypto;

    public HashService(ApplicationOptions options)
    {
        var hashingOptions = options.Clouded.Auth.Hash;

        if (hashingOptions.Argon2 != null)
            _crypto = new CryptoArgon2(hashingOptions.Argon2);

        if (_crypto == null)
            throw new NullReferenceException();
    }

    public string Hash(string password, string salt) => _crypto.Hash(password, salt);

    public bool Verify(string hash, string password, string salt) =>
        _crypto.Verify(hash, password, salt);
}
