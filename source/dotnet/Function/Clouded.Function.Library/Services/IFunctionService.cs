using Clouded.Function.Framework.Contexts.Base;
using Clouded.Function.Library.Dtos;
using Clouded.Function.Shared;

namespace Clouded.Function.Library.Services;

public interface IFunctionService
{
    public Task<ExecutionResponse> Execute(
        string executablePath,
        string name,
        EFunctionType type,
        HookContext? context
    );
}
