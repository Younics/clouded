using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class EntityIdentityPermissionOptions : BaseEntityIdentityOptions
{
    public readonly EntityMetaOptions Meta = new();
    public readonly EntitySupportOptions Support = new();
}
