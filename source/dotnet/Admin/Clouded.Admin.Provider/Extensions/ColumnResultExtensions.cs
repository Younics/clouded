using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Shared;
using Clouded.Shared.Enums;

namespace Clouded.Admin.Provider.Extensions;

public static class ColumnResultExtensions
{
    public static IEnumerable<ColumnResult> OnlySelectableFields(
        this IEnumerable<ColumnResult> columnResults,
        TableOptions? tableOptions
    )
    {
        return columnResults.Where(
            x =>
                tableOptions?.Columns.FirstOrDefault(i => i.Column == x.Name)?.Type
                != ETableColumnType.Password
        );
    }
}
