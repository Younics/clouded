namespace Clouded.Auth.Provider.Services.Interfaces;

public interface IHashService
{
    public string Hash(string password, string salt);
    public bool Verify(string hash, string password, string salt);
}
