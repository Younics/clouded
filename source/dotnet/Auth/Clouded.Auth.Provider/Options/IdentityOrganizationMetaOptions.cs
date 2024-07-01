using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class IdentityOrganizationMetaOptions : EntityMetaOptions
{
    public override string Table => "clouded_auth_support_organizations_meta";
    public override string ColumnRelatedEntityId => "organization_id";
}
