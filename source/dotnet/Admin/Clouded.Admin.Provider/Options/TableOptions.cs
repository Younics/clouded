namespace Clouded.Admin.Provider.Options;

public class TableOptions
{
    public required long DataSourceId { get; set; }
    public required string Schema { get; set; }
    public required string Table { get; set; }
    public required string Name { get; set; }
    public string? SingularName { get; set; }
    public required string? NavGroup { get; set; }
    public string Icon { get; set; } = "ListAlt";
    public required bool InMenu { get; set; }
    public required IEnumerable<TableColumnOptions> Columns { get; set; }

    #region Functions

    public OperationFunctionsOptions? CreateOperationFunctions { get; set; }
    public OperationFunctionsOptions? UpdateOperationFunctions { get; set; }
    public OperationFunctionsOptions? DeleteOperationFunctions { get; set; }

    #endregion
}
