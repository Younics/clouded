using System.Text;
using Clouded.Auth.Provider.Options;
using Clouded.Shared.Cryptography;
using Konscious.Security.Cryptography;

namespace Clouded.Auth.Provider.Cryptography
{
    public class CryptoArgon2(Argon2Options argon2Options) : ICrypto
    {
        public string Hash(string password, string? salt)
        {
            if (salt == null)
                throw new ArgumentNullException(nameof(salt));

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            var argon2 = new Argon2id(passwordBytes);
            argon2.Salt = saltBytes;
            argon2.DegreeOfParallelism = argon2Options.DegreeOfParallelism;
            argon2.Iterations = argon2Options.Iterations;
            argon2.MemorySize = argon2Options.MemorySize;
            var hashBytes = argon2.GetBytes(argon2Options.ReturnBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public bool Verify(string hash, string password, string? salt)
        {
            if (salt == null)
                throw new ArgumentNullException(nameof(salt));

            var verifyHash = Hash(password, salt);
            return hash.SequenceEqual(verifyHash);
        }
    }
}
