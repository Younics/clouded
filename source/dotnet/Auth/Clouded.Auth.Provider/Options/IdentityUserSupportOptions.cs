using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class IdentityUserSupportOptions : EntitySupportOptions
{
    public override string Table => "clouded_auth_support_users";
    public override string KeyPrefix => "cs_users";
    public override string ColumnRelatedEntityId => "user_id";
    public readonly string ColumnFbAccessCode = "fb_access_code";
    public readonly string ColumnGoogleAccessCode = "google_access_code";
    public readonly string ColumnAppleAccessCode = "apple_access_code";
}
