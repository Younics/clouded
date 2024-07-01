using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class IdentityMachineMetaOptions : EntityMetaOptions
{
    public override string Table => "clouded_auth_support_machines_meta";
    public override string ColumnRelatedEntityId => "machine_id";
}
