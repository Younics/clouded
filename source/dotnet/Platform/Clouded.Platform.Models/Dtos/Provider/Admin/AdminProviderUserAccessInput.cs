namespace Clouded.Platform.Models.Dtos.Provider.Admin;

public class AdminProviderUserAccessInput
{
    public long? Id { get; set; }
    public long AdminProviderId { get; set; }
    public string Identity { get; set; }
    public string Password { get; set; }
}
