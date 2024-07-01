namespace Clouded.Auth.Provider.Options.Base;

public class EntityMetaOptions
{
    public virtual string Table { get; set; }
    public virtual string ColumnKey { get; set; } = "key";
    public virtual string ColumnValue { get; set; } = "value";
    public virtual string ColumnRelatedEntityId { get; set; }
}
