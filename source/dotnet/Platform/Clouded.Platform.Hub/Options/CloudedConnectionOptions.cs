namespace Clouded.Platform.Hub.Options;

public class CloudedConnectionOptions
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }

    public string EncryptionKey { get; set; } = null!;
}
