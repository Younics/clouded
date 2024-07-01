using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Core.DataSource.Shared.Interfaces;
using Clouded.Shared.Enums;

namespace Clouded.Core.DataSource.Shared.Extensions;

public static class ColumnResultExtensions
{
    public static IEnumerable<ColumnResult> OnlyFilterableFields(
        this IEnumerable<ColumnResult> columnResults
    )
    {
        return columnResults.Where(
            i =>
                EColumnTypeGroups.Textual.Contains(i.Type)
                || EColumnTypeGroups.Numeric.Contains(i.Type)
                || i.Type == EColumnType.Boolean
                || i.Type == EColumnType.Date
                || i.Type == EColumnType.DateTime
                || i.Type == EColumnType.Time
        );
    }

    public static ICondition BuildFilterCondition(
        this ColumnResult column,
        string tableName,
        object value
    )
    {
        switch (column.Type)
        {
            case EColumnType.Boolean:
            case EColumnType.SmallSerial:
            case EColumnType.Serial:
            case EColumnType.Int:
            case EColumnType.BigSerial:
            case EColumnType.Long:
            case EColumnType.Double:
            case EColumnType.Decimal:
                return new ConditionValueInput
                {
                    Alias = tableName,
                    Column = column.Name,
                    Value = value,
                    Operator = EOperator.Equals
                };
            case EColumnType.Text:
            case EColumnType.Varchar:
                return new ConditionValueInput
                {
                    Alias = tableName,
                    Column = column.Name,
                    Value = value,
                    Operator = EOperator.Contains,
                    Mode = EMode.Insensitive
                };
            default:
                return ConditionValueInput.GetDefaultCondition();
        }
    }
}
