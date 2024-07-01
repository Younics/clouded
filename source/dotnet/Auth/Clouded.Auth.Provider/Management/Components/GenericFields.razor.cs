using Clouded.Auth.Provider.Management.Enums;
using Clouded.Core.DataSource.Shared;
using Microsoft.AspNetCore.Components;

namespace Clouded.Auth.Provider.Management.Components;

public partial class GenericFields<TDictionary>
    where TDictionary : DataSourceDictionary
{
    [Parameter]
    public required IEnumerable<ColumnResult> Columns { get; set; }

    [Parameter]
    public required TDictionary Data { get; set; }

    [Parameter]
    public EventCallback<TDictionary>? DataChanged { get; set; }

    [Parameter]
    public EGenericFieldMode Mode { get; set; }

    [Parameter]
    public string? IdColumnName { get; set; }

    [Parameter]
    public string? PasswordColumnName { get; set; }

    protected override void OnInitialized()
    {
        foreach (var column in Columns)
        {
            if (!Data.ContainsKey(column.Name))
                Data[column.Name] = default;
        }
    }

    private void SetValue(string columnName, object? columnValue)
    {
        Data[columnName] = columnValue;
        DataChanged?.InvokeAsync(Data);
    }
}
