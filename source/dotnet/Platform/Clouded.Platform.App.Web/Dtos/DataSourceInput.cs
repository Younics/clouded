using Clouded.Core.DataSource.Shared;

namespace Clouded.Platform.App.Web.Dtos;

public class DataSourceInput
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public DatabaseType? Type { get; set; }
    public string? Server { get; set; }
    public int? Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Database { get; set; }
    public long? ProjectId { get; set; }
}
