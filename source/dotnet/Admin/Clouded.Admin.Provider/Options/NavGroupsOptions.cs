namespace Clouded.Admin.Provider.Options;

public class NavGroupsOptions
{
    public required string Key { get; set; }
    public required string Label { get; set; }
    public required string Icon { get; set; } = "ListAlt";
    public required int Order { get; set; }
}
