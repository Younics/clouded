using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class EntityIdentityOrganizationOptions : BaseEntityIdentityOptions
{
    public readonly IdentityOrganizationMetaOptions Meta = new();
    public readonly EntitySupportOptions Support = new();
}
