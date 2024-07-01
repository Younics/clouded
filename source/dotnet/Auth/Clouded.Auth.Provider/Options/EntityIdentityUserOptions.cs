using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class EntityIdentityUserOptions : BaseEntityIdentityOptions
{
    public string ColumnPassword { get; set; } = null!;

    public readonly IdentityUserMetaOptions Meta = new();
    public readonly IdentityUserSupportOptions Support = new();
    public readonly IdentityUserRefreshTokenOptions RefreshToken = new();
    public required IdentityAppleDataOptions AppleData { get; set; }
}
