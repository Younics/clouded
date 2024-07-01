using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CliWrap;
using Clouded.Function.Framework.Contexts.Base;
using Clouded.Function.Library.Dtos;
using Clouded.Function.Shared;
using Clouded.Shared;
using Clouded.Shared.Enums;
using Serilog;

namespace Clouded.Function.Library.Services;

public class FunctionService : IFunctionService
{
    public async Task<ExecutionResponse> Execute(
        string executablePath,
        string functionName,
        EFunctionType eFunctionType,
        HookContext? context
    )
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        // TODO: More precise exception
        if (executablePath == null)
            throw new ArgumentOutOfRangeException();

        var contextJson = JsonSerializer.Serialize(context ?? new HookContext());

        Log.Information(
            "Execute: {FunctionName} {FunctionType} {Context}",
            functionName,
            eFunctionType,
            contextJson
        );

        var commandResult = await Cli.Wrap(executablePath)
            .WithArguments(new[] { functionName, eFunctionType.GetEnumName(), contextJson })
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync();

        var logs = stdOutBuffer.ToString();
        var logsMatch = Regex.Match(
            logs,
            $@"(?<=\[{ECloudedFunctionBlock.CloudedFunctionLogs}\])(.*)(?=\[\/{ECloudedFunctionBlock.CloudedFunctionLogs}\])",
            RegexOptions.Singleline
        );

        var dataMatch = Regex.Match(
            logs,
            $@"(?<=\[{ECloudedFunctionBlock.CloudedFunctionData}\])(.*)(?=\[\/{ECloudedFunctionBlock.CloudedFunctionData}\])",
            RegexOptions.Singleline
        );

        var errorMatch = Regex.Match(
            logs,
            $@"(?<=\[{ECloudedFunctionBlock.CloudedFunctionError}\])(.*)(?=\[\/{ECloudedFunctionBlock.CloudedFunctionError}\])",
            RegexOptions.Singleline
        );

        var response = new ExecutionResponse
        {
            Success = commandResult.ExitCode == 0 && !errorMatch.Success,
            Logs = logsMatch.Success ? logsMatch.Value : null,
            Data = dataMatch.Success ? dataMatch.Value : null,
            Error = errorMatch.Success ? errorMatch.Value : null
        };

        Log.Information("Execution Logs: {Logs}", response.Logs);
        Log.Information("Execution Data: {Data}", response.Data);
        Log.Error("Execution Error: {Error}", response.Error);

        return response;
    }
}
