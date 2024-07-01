using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Shared.Input;

public class SelectColDesc
{
    public IEnumerable<string> ColJoin { get; init; } = Array.Empty<string>();
    public string? Alias { get; init; }
}

public class SelectInput
{
    public required string Schema { get; init; }
    public required string Table { get; init; }
    public required string Alias { get; init; }
    public IEnumerable<SelectColDesc> SelectedColumns { get; init; } = Array.Empty<SelectColDesc>();

    public string SelectedColumnsRaw
    {
        get
        {
            if (SelectedColumns.Any())
                return string.Join(
                    ',',
                    SelectedColumns.Select(column =>
                    {
                        var colDesc = string.Join(
                            '.',
                            column.ColJoin.Select(
                                columnProperty =>
                                    columnProperty == "*"
                                        ? columnProperty
                                        : $""" "{columnProperty}" """
                            )
                        );

                        if (column.Alias != null)
                        {
                            colDesc += $"\"{column.Alias}\"";
                        }

                        return colDesc;
                    })
                );

            return "*";
        }
    }

    public bool Distinct { get; set; }

    public IEnumerable<JoinInput> Join { get; set; } = Array.Empty<JoinInput>();

    public ICondition? Where { get; set; }

    public IEnumerable<GroupByInput> GroupBy { get; set; } = Array.Empty<GroupByInput>();
    public IEnumerable<OrderInput> OrderBy { get; set; } = Array.Empty<OrderInput>();

    public int? Offset { get; init; }
    public int? Limit { get; init; }
}
