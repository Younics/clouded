namespace Clouded.Shared.Cryptography;

public interface ICrypto
{
    public string Hash(string password, string? salt = null);
    public bool Verify(string hash, string password, string? salt = null);
}
