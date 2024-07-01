namespace Clouded.Auth.Provider.Options.Base;

public abstract class EntityOptions
{
    public string Schema { get; set; } = null!;
    public virtual string Table { get; set; } = null!;
    public virtual string ColumnId { get; set; } = null!;
}
