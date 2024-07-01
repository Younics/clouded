namespace Clouded.Admin.Provider.Options;

public class AdminOptions
{
    public required string Name { get; set; }

    public required AuthOptions Auth { get; set; }

    public required IEnumerable<TableOptions> Tables { get; set; }
    public required IEnumerable<NavGroupsOptions> NavGroups { get; set; } =
        new List<NavGroupsOptions>();

    #region Functions

    public List<FunctionProviderOptions> FunctionProviders { get; set; } = new();
    public OperationFunctionsOptions? GlobalCreateOperationFunctions { get; set; }
    public OperationFunctionsOptions? GlobalUpdateOperationFunctions { get; set; }
    public OperationFunctionsOptions? GlobalDeleteOperationFunctions { get; set; }

    #endregion
}
