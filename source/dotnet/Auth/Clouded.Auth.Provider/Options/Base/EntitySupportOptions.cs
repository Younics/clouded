namespace Clouded.Auth.Provider.Options.Base;

public class EntitySupportOptions
{
    public virtual string Table { get; set; } = null!;
    public virtual string KeyPrefix { get; set; } = null!;
    public virtual string ColumnRelatedEntityId { get; set; } = null!;

    public readonly string ColumnBlocked = "blocked";
    public readonly string ColumnAdminAccess = "admin_access";
    public readonly string ColumnPasswordResetToken = "password_reset_token";
    public readonly string ColumnSalt = "salt";
}
