namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderInput
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string CodePrefix { get; set; } = null!;
    public string? Code { get; set; }
    public long? DomainId { get; set; }
    public long? DataSourceProviderId { get; set; }
    public long? ProjectId { get; set; }

    public AuthProviderConfigurationInput Configuration { get; set; } = new();
}
