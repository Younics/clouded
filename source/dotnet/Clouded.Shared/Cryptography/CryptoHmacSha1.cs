using System.Security.Cryptography;
using System.Text;

namespace Clouded.Shared.Cryptography
{
    public class CryptoHmacSha1 : ICrypto
    {
        public string Hash(string password, string? salt = null)
        {
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var saltBytes = Encoding.ASCII.GetBytes(salt!);

            var hmacSha1 = new HMACSHA1(saltBytes);
            var hashBytes = hmacSha1.ComputeHash(passwordBytes);

            return string.Concat(Array.ConvertAll(hashBytes, x => x.ToString("x2")));
        }

        public bool Verify(string hash, string password, string? salt)
        {
            throw new NotImplementedException();
        }
    }
}
