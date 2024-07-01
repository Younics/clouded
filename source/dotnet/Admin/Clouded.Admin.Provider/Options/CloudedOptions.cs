namespace Clouded.Admin.Provider.Options;

public class CloudedOptions
{
    public required IEnumerable<DataSourceOptions> DataSources { get; set; }
    public required AdminOptions Admin { get; set; }
}
