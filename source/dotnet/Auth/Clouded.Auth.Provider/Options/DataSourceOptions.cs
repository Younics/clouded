using Clouded.Core.DataSource.Shared;

namespace Clouded.Auth.Provider.Options;

public class DataSourceOptions
{
    public DatabaseType Type { get; set; }
    public string Server { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
}
