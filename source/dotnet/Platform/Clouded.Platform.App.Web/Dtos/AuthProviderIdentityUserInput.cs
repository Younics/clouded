namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderIdentityUserInput : AuthProviderIdentityInput
{
    public string? ColumnPassword { get; set; }

    public AuthProviderIdentityUserInput()
    {
        Enabled = true;
    }
}
