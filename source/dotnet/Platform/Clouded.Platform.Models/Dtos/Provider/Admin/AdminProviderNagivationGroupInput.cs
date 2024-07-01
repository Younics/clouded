namespace Clouded.Platform.Models.Dtos.Provider.Admin;

public class AdminProviderNavigationGroupInput
{
    public long? Id { get; set; }
    public long AdminProviderId { get; set; }
    public string Key { get; set; }
    public string Label { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
}
