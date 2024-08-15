using Clouded.Shared.Enums;

namespace Clouded.Core.DataSource.Shared;

public class ColumnResult
{
    public string SchemaName { get; init; } = null!;
    public string TableName { get; init; } = null!;
    public string Name { get; init; } = null!;
    public EColumnType Type { get; init; }
    public string? TypeRaw { get; init; }
    public int Position { get; set; }
    public bool IsNullable { get; init; }
    public bool IsPrimary { get; init; }
    public bool IsAutoIncrement { get; init; }
    public bool IsGenerated { get; init; }
    public int? MaxLength { get; init; }
    public bool IsForeignKey { get; init; }
    public RelationResult? InsideRelation { get; init; }

    public bool IsFilterable()
    {
        return Type
            is EColumnType.Boolean
                or EColumnType.Varchar
                or EColumnType.Text
                or EColumnType.SmallSerial
                or EColumnType.Serial
                or EColumnType.Int
                or EColumnType.BigSerial
                or EColumnType.Long
                or EColumnType.Double
                or EColumnType.Decimal;
    }
}
