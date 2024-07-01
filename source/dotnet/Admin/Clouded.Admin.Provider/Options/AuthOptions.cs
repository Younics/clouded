namespace Clouded.Admin.Provider.Options;

public class AuthOptions
{
    public required string TokenKey { get; set; }
    public required string IdentityKey { get; set; }
    public required string PasswordKey { get; set; }
    public required UserSettingsOptions UserSettings { get; set; }

    public required IEnumerable<AuthUserOptions> Users { get; set; } =
        Array.Empty<AuthUserOptions>();
}
