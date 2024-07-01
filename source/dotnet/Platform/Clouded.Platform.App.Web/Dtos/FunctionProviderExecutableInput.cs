using Clouded.Function.Shared;

namespace Clouded.Platform.App.Web.Dtos;

public class FunctionProviderExecutableInput
{
    public string ExecutableName { get; set; } = null!;
    public string FunctionName { get; set; } = null!;
    public EFunctionType FunctionType { get; set; }
}
