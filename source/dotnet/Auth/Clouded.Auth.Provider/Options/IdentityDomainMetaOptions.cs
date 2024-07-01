using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class IdentityDomainMetaOptions : EntityMetaOptions
{
    public override string Table => "clouded_auth_support_domains_meta";
    public override string ColumnRelatedEntityId => "domain_id";
}
