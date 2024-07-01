namespace Clouded.Platform.Hub.Options;

public class ContainerConnectionOptions
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }

    public string ConnectionString =>
        $"Server={Server};"
        + $"Port={Port};"
        + $"Database={Database};"
        + $"User Id={Username};"
        + $"Password={Password}";
}
