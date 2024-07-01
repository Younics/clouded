namespace Clouded.Admin.Provider.Options;

public class UserSettingsOptions
{
    public required long DataSourceId { get; set; }
    public required string Schema { get; set; }
    public required string Table { get; set; }
    public required string ColumnId { get; set; }
    public required string ColumnSettings { get; set; }
}
