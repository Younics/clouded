namespace Clouded.Shared.Cryptography
{
    public class CryptoAdler32 : ICrypto
    {
        public string Hash(string password, string? salt = null)
        {
            const int mod = 65521;
            uint a = 1,
                b = 0;

            foreach (var c in password)
            {
                a = (a + c) % mod;
                b = (b + a) % mod;
            }

            return ((b << 16) | a).ToString();
        }

        public bool Verify(string hash, string password, string? salt)
        {
            throw new NotImplementedException();
        }
    }
}
