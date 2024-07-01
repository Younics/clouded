namespace Clouded.Platform.Models.Dtos.Provider.Admin;

public class AdminProviderInput
{
    public long? Id { get; set; }
    public string? Name { get; set; }

    public string? CodePrefix { get; set; }

    public string? Code { get; set; }
    public IEnumerable<long> DataSourceProviderIds { get; set; } = Array.Empty<long>();

    public long? DomainId { get; set; }
    public long? ProjectId { get; set; }

    public AdminProviderFunctionsBlockInput CreateFunctions { get; set; } = new();
    public AdminProviderFunctionsBlockInput UpdateFunctions { get; set; } = new();
    public AdminProviderFunctionsBlockInput DeleteFunctions { get; set; } = new();

    public AdminProviderUserSettingsInput UserSettings { get; set; } = new();

    public IEnumerable<AdminProviderTableInput> Tables { get; set; } =
        new List<AdminProviderTableInput>();

    public IEnumerable<AdminProviderNavigationGroupInput> NavGroups { get; set; } =
        new List<AdminProviderNavigationGroupInput> { };

    public IEnumerable<AdminProviderUserAccessInput> UserAccess { get; set; } =
        new List<AdminProviderUserAccessInput> { };
}
