namespace Clouded.Shared.Enums;

public enum EColumnType
{
    Unsupported,

    Boolean,

    Char,
    Varchar,
    Text,

    Int,
    Long,

    Double,
    Decimal,

    Time,
    Date,
    DateTime,

    SmallSerial,
    Serial,
    BigSerial,

    Bytea
}

public static class EColumnTypeGroups
{
    public static readonly EColumnType[] Textual =
    {
        EColumnType.Varchar,
        EColumnType.Char,
        EColumnType.Text
    };
    public static readonly EColumnType[] Numeric =
    {
        EColumnType.Int,
        EColumnType.Long,
        EColumnType.Double,
        EColumnType.Decimal
    };
}
