using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class IdentityUserMetaOptions : EntityMetaOptions
{
    public override string Table => "clouded_auth_support_users_meta";
    public override string ColumnRelatedEntityId => "user_id";
}
