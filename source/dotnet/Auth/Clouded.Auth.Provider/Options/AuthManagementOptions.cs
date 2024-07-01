namespace Clouded.Auth.Provider.Options;

public class AuthManagementOptions
{
    public required bool Enabled { get; set; }

    public required string TokenKey { get; set; }
    public required string IdentityKey { get; set; }
    public required string PasswordKey { get; set; }

    public required IEnumerable<AuthManagementUserOptions> Users { get; set; } =
        Array.Empty<AuthManagementUserOptions>();
}
