namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderManagementInput
{
    public bool Enabled { get; set; }

    public List<AuthProviderManagementUserInput> Users { get; set; } = new();
}
