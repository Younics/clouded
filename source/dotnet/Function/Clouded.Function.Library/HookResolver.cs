using System.Text.Json;
using Clouded.Function.Framework.Contexts;
using Clouded.Function.Framework.Contexts.Base;
using Clouded.Function.Framework.Outputs;
using Clouded.Function.Library.Dtos;
using Clouded.Function.Library.Options;
using Clouded.Function.Shared;
using Flurl;
using Flurl.Http;

namespace Clouded.Function.Library;

public class HookResolver(FunctionOptions? options)
{
    public bool IsHookEnabled(string providerHookName, EFunctionType type) =>
        options != null && ResolveHookMethod(providerHookName, type).Any();

    public async Task<(bool Success, List<ValidationOutput> Data, string? Error)> Validation(
        string providerHookName,
        ValidationContext context,
        CancellationToken cancellationToken = default
    )
    {
        var responses = await ResolveHooksAsync(
            providerHookName,
            EFunctionType.ValidationHook,
            context,
            cancellationToken
        );

        return (
            Success: responses.All(response => response.Success),
            Data: responses
                .Select(
                    x =>
                        JsonSerializer.Deserialize<ValidationOutput>(
                            JsonSerializer.Serialize(x.Data)
                        )
                )
                .ToList(),
            Error: ResolveError(responses)
        )!;
    }

    public async Task<(object? Data, string? Error)> TransformInput(
        string providerHookName,
        HookContext context,
        CancellationToken cancellationToken = default
    )
    {
        var responses = await ResolveHooksAsync(
            providerHookName,
            EFunctionType.InputHook,
            context,
            cancellationToken
        );

        return (responses.LastOrDefault()?.Data, Error: ResolveError(responses));
    }

    public async Task<(object? Data, string? Error)> TransformOutput(
        string providerHookName,
        HookContext context,
        CancellationToken cancellationToken = default
    )
    {
        var responses = await ResolveHooksAsync(
            providerHookName,
            EFunctionType.OutputHook,
            context,
            cancellationToken
        );

        return (responses.LastOrDefault()?.Data, Error: ResolveError(responses));
    }

    public async Task HookBefore(
        string providerHookName,
        HookContext context,
        CancellationToken cancellationToken = default
    ) =>
        await ResolveHooksAsync(
            providerHookName,
            EFunctionType.BeforeHook,
            context,
            cancellationToken
        );

    public async Task HookAfter(
        string providerHookName,
        HookContext context,
        CancellationToken cancellationToken = default
    ) =>
        await ResolveHooksAsync(
            providerHookName,
            EFunctionType.AfterHook,
            context,
            cancellationToken
        );

    private IEnumerable<FunctionHookMethodOptions> ResolveHookMethod(
        string providerHookName,
        EFunctionType hookType
    )
    {
        if (options == null)
            return Array.Empty<FunctionHookMethodOptions>();

        return hookType switch
        {
            EFunctionType.ValidationHook => options.Hooks[providerHookName].Validation,
            EFunctionType.InputHook => options.Hooks[providerHookName].TransformInput,
            EFunctionType.OutputHook => options.Hooks[providerHookName].TransformOutput,
            EFunctionType.BeforeHook => options.Hooks[providerHookName].HookBefore,
            EFunctionType.AfterHook => options.Hooks[providerHookName].HookAfter,
            _ => Array.Empty<FunctionHookMethodOptions>()
        };
    }

    private string? ResolveError(IEnumerable<ExecutionResponse> responses)
    {
        var errors = responses.Where(x => x.Error != null).Select(x => x.Error!).ToList();

        return errors.Any() ? string.Join("\n", errors) : null;
    }

    private async Task<List<ExecutionResponse>> ResolveHooksAsync(
        string providerHookName,
        EFunctionType type,
        HookContext context,
        CancellationToken cancellationToken = default
    )
    {
        var hooks = ResolveHookMethod(providerHookName, type);

        var responses = new List<ExecutionResponse>();

        foreach (var hook in hooks.OrderBy(x => x.Index))
        {
            var provider = options!.Providers.Single(x => x.Id == hook.ProviderId);

            var response = await RequestAsync(
                provider.Url,
                provider.ApiKey,
                hook.Name,
                type,
                context,
                cancellationToken
            );

            if (response == null)
                continue;

            if (response.Success && type is EFunctionType.InputHook or EFunctionType.OutputHook)
                context.Data = response.Data;

            responses.Add(response);
        }

        return responses;
    }

    private static async Task<ExecutionResponse?> RequestAsync(
        string providerUrl,
        string providerApiKey,
        string hookName,
        EFunctionType hookType,
        HookContext hookContext,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await providerUrl
                .AppendPathSegment("v1/execute/hook")
                .WithHeader("X-CLOUDED-KEY", providerApiKey)
                .PostJsonAsync(
                    new HookRequest
                    {
                        Name = hookName,
                        Type = hookType,
                        Context = hookContext
                    },
                    HttpCompletionOption.ResponseContentRead,
                    cancellationToken
                )
                .ReceiveJson<ExecutionResponse?>();
        }
        catch
        {
            return null;
        }
    }
}
