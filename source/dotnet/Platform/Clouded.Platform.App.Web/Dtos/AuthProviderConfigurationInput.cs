namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderConfigurationInput
{
    public long? Id { get; set; }
    public bool Documentation { get; set; }
    public AuthProviderCorsInput Cors { get; set; } = new();
    public AuthProviderHashInput Hash { get; set; } = new();
    public AuthProviderArgonHashInput ArgonInput { get; set; } = new();
    public AuthProviderTokenInput Token { get; set; } = new();
    public AuthProviderSocialInput Google { get; set; } = new();
    public AuthProviderSocialInput Facebook { get; set; } = new();
    public AuthProviderSocialInput Apple { get; set; } = new();
    public AuthProviderIdentityInput IdentityOrganization { get; set; } = new();
    public AuthProviderIdentityUserInput IdentityUser { get; set; } = new();
    public AuthProviderIdentityInput IdentityRole { get; set; } = new();
    public AuthProviderIdentityInput IdentityPermission { get; set; } = new();
    public AuthProviderManagementInput Management { get; set; } = new();
    public AuthProviderMailInput Mail { get; set; } = new();
}
