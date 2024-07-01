using Clouded.Auth.Provider.Options.Base;

namespace Clouded.Auth.Provider.Options;

public class EntityIdentityMachineOptions : BaseEntityIdentityOptions
{
    public override string Table { get; set; } = "clouded_auth_support_machines";
    public override string ColumnId { get; set; } = "id";
    public override string ColumnIdentity { get; set; } = "name";

    public readonly string KeyPrefix = "cs_machines";
    public string ColumnDescription = "description";
    public string ColumnApiKey = "api_key";
    public string ColumnSecretKey = "secret_key";
    public string ColumnExpiresIn = "expires_in";
    public string ColumnBlocked = "blocked";
    public string ColumnType = "type";

    public readonly IdentityMachineMetaOptions Meta = new();
    public readonly EntitySupportOptions Support = new();
}
