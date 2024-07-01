using System.Text.Json;
using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Shared;
using Clouded.Function.Framework.Contexts;
using Clouded.Function.Framework.Contexts.Base;
using Clouded.Function.Framework.Outputs;
using Clouded.Function.Library.Dtos;
using Clouded.Function.Library.Services;
using Clouded.Function.Shared;
using Clouded.Shared.Exceptions;

namespace Clouded.Admin.Provider.Extensions;

public static class FunctionExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="functionService"></param>
    /// <param name="functionProviders"></param>
    /// <param name="globalOperationFunctions"></param>
    /// <param name="localOperationFunctions"></param>
    /// <returns></returns>
    public static async Task<List<ValidationOutput>> ExecuteValidationFunctions(
        this ValidationContext ctx,
        IFunctionService functionService,
        List<FunctionProviderOptions> functionProviders,
        OperationFunctionsOptions? globalOperationFunctions,
        OperationFunctionsOptions? localOperationFunctions
    )
    {
        var validationOutputs = new List<ValidationOutput>();

        if (globalOperationFunctions?.ValidationHooks != null)
        {
            validationOutputs.AddRange(
                await globalOperationFunctions.ValidationHooks.ExecuteValidations(
                    functionProviders,
                    functionService,
                    ctx
                )
            );
        }

        if (localOperationFunctions?.ValidationHooks != null)
        {
            validationOutputs.AddRange(
                await localOperationFunctions.ValidationHooks.ExecuteValidations(
                    functionProviders,
                    functionService,
                    ctx
                )
            );
        }

        return validationOutputs;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="functions"></param>
    /// <param name="hookType"></param>
    /// <param name="functionProviders"></param>
    /// <param name="functionService"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="FunctionExecutionException"></exception>
    public static async Task ExecuteVoidHooks(
        this List<FunctionOptions> functions,
        EFunctionType hookType,
        List<FunctionProviderOptions> functionProviders,
        IFunctionService functionService,
        HookContext context
    )
    {
        if (hookType is EFunctionType.InputHook or EFunctionType.OutputHook)
        {
            throw new OperationCanceledException("Use ExecuteDataHooks function instead");
        }

        foreach (var hook in functions)
        {
            var response = await hook.ExecuteFunction(
                hookType,
                functionProviders,
                functionService,
                context
            );

            if (response.Error != null)
            {
                throw new FunctionExecutionException(response.Error);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="functions"></param>
    /// <param name="hookType"></param>
    /// <param name="functionProviders"></param>
    /// <param name="functionService"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="FunctionExecutionException"></exception>
    public static async Task<DataSourceDictionary> ExecuteDataHooks(
        this List<FunctionOptions> functions,
        EFunctionType hookType,
        List<FunctionProviderOptions> functionProviders,
        IFunctionService functionService,
        HookContext context
    )
    {
        if (hookType is EFunctionType.AfterHook or EFunctionType.BeforeHook)
        {
            throw new OperationCanceledException("Use ExecuteVoidHooks function instead");
        }

        foreach (var hook in functions)
        {
            var response = await hook.ExecuteFunction(
                hookType,
                functionProviders,
                functionService,
                context
            );

            if (response.Error != null)
            {
                throw new FunctionExecutionException(response.Error);
            }

            if (response.Data != null)
            {
                context.Data = JsonSerializer.Deserialize<DataSourceDictionary>(response.Data);
            }
        }

        return (DataSourceDictionary)context.Data;
    }

    private static async Task<List<ValidationOutput>> ExecuteValidations(
        this List<FunctionOptions> functions,
        IReadOnlyCollection<FunctionProviderOptions> functionProviders,
        IFunctionService functionService,
        ValidationContext context
    )
    {
        List<ValidationOutput> validationOutputs = new List<ValidationOutput>();

        foreach (var hook in functions)
        {
            var response = await hook.ExecuteFunction(
                EFunctionType.ValidationHook,
                functionProviders,
                functionService,
                context
            );

            if (response.Data != null)
            {
                validationOutputs.Add(JsonSerializer.Deserialize<ValidationOutput>(response.Data)!);
            }
            else if (response.Error != null)
            {
                throw new FunctionExecutionException(response.Error);
            }
        }

        return validationOutputs;
    }

    private static async Task<ExecutionResponse> ExecuteFunction(
        this FunctionOptions function,
        EFunctionType functionType,
        IEnumerable<FunctionProviderOptions> functionProviders,
        IFunctionService functionService,
        HookContext? context = default
    )
    {
        var fncProvider = functionProviders.FirstOrDefault(i => i.Id == function.SourceId);

        if (fncProvider == null)
        {
            throw new FunctionProviderNotFoundException(function.Name);
        }

        return await functionService.Execute(
            fncProvider.ExecuteCmd,
            function.Name,
            functionType,
            context
        );
    }
}
