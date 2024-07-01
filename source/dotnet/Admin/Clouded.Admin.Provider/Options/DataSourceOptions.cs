using Clouded.Core.DataSource.Shared;

namespace Clouded.Admin.Provider.Options;

public class DataSourceOptions
{
    public required long Id { get; set; }
    public required DatabaseType Type { get; set; }
    public required string Server { get; set; }
    public required int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Database { get; set; }
}
