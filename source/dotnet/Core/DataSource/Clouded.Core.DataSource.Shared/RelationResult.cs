namespace Clouded.Core.DataSource.Shared;

public class RelationResult : IEquatable<RelationResult>
{
    public required string Schema { get; init; }
    public required string Table { get; init; }
    public required string Column { get; init; }
    public required bool ColumnNotNull { get; init; }
    public required string TargetSchema { get; init; }
    public required string TargetTable { get; init; }
    public required string TargetColumn { get; init; }
    public required bool TargetColumnNotNull { get; init; }
    public required bool IsUnique { get; init; }

    public string GetAlias()
    {
        return TargetTable + "_" + Column;
    }

    public bool Equals(RelationResult? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Schema == other.Schema
            && Table == other.Table
            && Column == other.Column
            && ColumnNotNull == other.ColumnNotNull
            && TargetSchema == other.TargetSchema
            && TargetTable == other.TargetTable
            && TargetColumn == other.TargetColumn
            && TargetColumnNotNull == other.TargetColumnNotNull
            && IsUnique == other.IsUnique;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((RelationResult)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Schema);
        hashCode.Add(Table);
        hashCode.Add(Column);
        hashCode.Add(ColumnNotNull);
        hashCode.Add(TargetSchema);
        hashCode.Add(TargetTable);
        hashCode.Add(TargetColumn);
        hashCode.Add(TargetColumnNotNull);
        hashCode.Add(IsUnique);
        return hashCode.ToHashCode();
    }
}

public enum RelationType
{
    OneToOne,
    OneToMany,
    ManyToOne,
    ManyToMany
}
