using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class EntityIdentityDomainOptions : BaseEntityIdentityOptions
{
    public override string Table { get; set; } = "clouded_auth_support_domains";
    public override string ColumnId { get; set; } = "id";
    public override string ColumnIdentity { get; set; } = "value";

    public readonly string KeyPrefix = "cs_domains";

    public readonly EntitySupportOptions Support = new();
}
