using Clouded.Shared.Enums;
using DynamicExpresso;

namespace Clouded.Admin.Provider.Options;

public class TableColumnOptions
{
    public required string Column { get; set; }
    public required string Name { get; set; }
    public required int Order { get; set; }
    public bool Filterable { get; set; }
    public string? VirtualValue { get; set; }
    public EVirtualColumnType VirtualType { get; set; } = EVirtualColumnType.String;
    public ETableColumnType Type { get; set; } = ETableColumnType.Inherits;
    public TableColumnValidation? ValidationType { get; set; }

    /// <summary>
    /// Used when <see cref="ValidationType"/> is <see cref="TableColumnValidation.Regex"/>
    /// </summary>
    public string? ValidationRegex { get; set; }

    /// <summary>
    /// Used when <see cref="ValidationType"/> is <see cref="TableColumnValidation.LessThan"/>, <see cref="TableColumnValidation.GreaterThan"/>,
    /// <see cref="TableColumnValidation.GreaterThanOrEqual"/> or <see cref="TableColumnValidation.LessThanOrEqual"/>
    /// </summary>
    public float? ValidationValue { get; set; }

    public required TableColumnListOptions List { get; set; }
    public required TableColumnDetailOptions Detail { get; set; }

    /// <summary>
    /// Used also for update
    /// </summary>
    public required TableColumnCreateOptions Create { get; set; }
}

public static class TableColumnOptionsExtensions
{
    public static IEnumerable<TableColumnOptions> OnlyVisibleForList(
        this IEnumerable<TableColumnOptions> columnOptionsEnumerable
    )
    {
        return columnOptionsEnumerable.Where(
            x =>
                x.List.Visible
                && x.Type != ETableColumnType.Password
                && x.Type != ETableColumnType.File
        );
    }

    public static bool IsSortable(this TableColumnOptions columnOptions)
    {
        return columnOptions.Type != ETableColumnType.File
            && columnOptions.Type != ETableColumnType.Image
            && columnOptions.Type != ETableColumnType.Virtual;
    }

    public static string GetVirtualFieldValue(
        this TableColumnOptions columnOptions,
        Dictionary<string, object> data
    )
    {
        if (columnOptions.Type != ETableColumnType.Virtual || columnOptions.VirtualValue == null)
        {
            return String.Empty;
        }

        var interpreter = new Interpreter();
        var parameters = data.Select(x => new Parameter(x.Key, x.Value ?? "")).ToArray();

        try
        {
            return interpreter.Eval(columnOptions.VirtualValue ?? "", parameters).ToString()
                ?? string.Empty;
        }
        catch (Exception e)
        {
            return "invalid expression";
        }
    }
}

public enum TableColumnValidation
{
    Regex,
    CreditCard,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
}
