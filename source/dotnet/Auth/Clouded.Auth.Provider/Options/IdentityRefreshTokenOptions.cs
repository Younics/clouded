namespace Clouded.Auth.Provider.Options;

public class IdentityUserRefreshTokenOptions
{
    public readonly string Table = "clouded_auth_support_refresh_tokens";
    public readonly string KeyPrefix = "cs_users";

    public readonly string ColumnToken = "token";
    public readonly string ColumnExpires = "expires";
    public readonly string ColumnUserId = "user_id";
}
