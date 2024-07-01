namespace Clouded.Admin.Provider.Options;

public class OperationFunctionsOptions
{
    public List<FunctionOptions>? BeforeHooks { get; set; }
    public List<FunctionOptions>? AfterHooks { get; set; }
    public List<FunctionOptions>? ValidationHooks { get; set; }
    public List<FunctionOptions>? InputHooks { get; set; }
}
