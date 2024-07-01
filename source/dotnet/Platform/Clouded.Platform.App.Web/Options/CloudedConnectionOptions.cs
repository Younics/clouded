namespace Clouded.Platform.App.Web.Options;

public class CloudedConnectionOptions
{
    public string Server { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Database { get; set; } = null!;

    public string EncryptionKey { get; set; } = null!;
}
