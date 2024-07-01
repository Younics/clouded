namespace Clouded.Auth.Provider.Options.Base;

public class BaseEntityIdentityOptions : EntityOptions
{
    public virtual string ColumnIdentity { get; set; } = null!;

    public string[] ColumnIdentityArray =>
        ColumnIdentity?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
}
