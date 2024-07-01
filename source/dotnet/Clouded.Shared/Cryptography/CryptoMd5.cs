using System.Security.Cryptography;
using System.Text;

namespace Clouded.Shared.Cryptography
{
    public class CryptoMd5 : ICrypto
    {
        public string Hash(string password, string? salt = null)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var md5Hash = MD5.Create();
            var hashBytes = md5Hash.ComputeHash(passwordBytes);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }

        public bool Verify(string hash, string password, string? salt)
        {
            throw new NotImplementedException();
        }
    }
}
