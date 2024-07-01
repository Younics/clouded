using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text.Json;
using Clouded.Function.Framework.Attributes;
using Clouded.Function.Framework.Components;
using Clouded.Function.Framework.Contexts;
using Clouded.Function.Framework.Contexts.Base;
using Clouded.Function.Shared;
using Clouded.Shared.Enums;

namespace Clouded.Function.Framework;

public static class CloudedFramework
{
    public static async void Use(string[] args)
    {
        try
        {
            Console.Write($"[{ECloudedFunctionBlock.CloudedFunctionLogs}]");

            var functionName = args[0];

            if (!Enum.TryParse<EFunctionType>(args[1], out var functionType))
                throw new InvalidEnumArgumentException("FUNCTION_TYPE_MISSING");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var @class = assemblies
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t =>
                {
                    if (!t.IsDefined(typeof(CloudedMapAttribute)))
                        return false;

                    var attribute = (CloudedMapAttribute)
                        Attribute.GetCustomAttribute(t, typeof(CloudedMapAttribute))!;

                    return attribute.Name == functionName
                        && t.IsSubclassOf(
                            functionType switch
                            {
                                EFunctionType.ValidationHook => typeof(ValidationHook),
                                EFunctionType.InputHook => typeof(InputHook),
                                EFunctionType.OutputHook => typeof(OutputHook),
                                EFunctionType.BeforeHook => typeof(BeforeHook),
                                EFunctionType.AfterHook => typeof(InputHook),
                                _ => typeof(Components.Base.Function)
                            }
                        );
                });

            if (@class == null)
                throw new NotImplementedException(functionName);

            object? data = null;

            switch (functionType)
            {
                case EFunctionType.BeforeHook:
                {
                    var hook = Activator.CreateInstance(@class) as BeforeHook;
                    var context = ParseContext<HookContext>(args);
                    await hook!.RunAsync(context!);
                    break;
                }
                case EFunctionType.AfterHook:
                {
                    var hook = Activator.CreateInstance(@class) as AfterHook;
                    var context = ParseContext<HookContext>(args);
                    await hook!.RunAsync(context!);
                    break;
                }
                case EFunctionType.InputHook:
                {
                    var hook = Activator.CreateInstance(@class) as InputHook;
                    var context = ParseContext<HookContext>(args);
                    data = await hook!.RunAsync(context!);
                    break;
                }
                case EFunctionType.OutputHook:
                {
                    var hook = Activator.CreateInstance(@class) as OutputHook;
                    var context = ParseContext<HookContext>(args);
                    data = await hook!.RunAsync(context!);
                    break;
                }
                case EFunctionType.ValidationHook:
                {
                    var hook = Activator.CreateInstance(@class) as ValidationHook;
                    var context = ParseContext<ValidationContext>(args);
                    data = await hook!.RunAsync(context!);
                    break;
                }
                case EFunctionType.Function:
                default:
                {
                    var function = Activator.CreateInstance(@class) as Components.Base.Function;
                    var context = ParseContext<FunctionContext>(args);
                    data = await function!.RunAsync(context!);
                    break;
                }
            }

            Console.Write($"[/{ECloudedFunctionBlock.CloudedFunctionLogs}]");

            Console.Write($"[{ECloudedFunctionBlock.CloudedFunctionData}]");
            Console.Write(JsonSerializer.Serialize(data));
            Console.Write($"[/{ECloudedFunctionBlock.CloudedFunctionData}]");
        }
        catch (Exception ex)
        {
            Console.Write($"[/{ECloudedFunctionBlock.CloudedFunctionLogs}]");

            Console.Write($"[{ECloudedFunctionBlock.CloudedFunctionError}]");
            Console.Write(ex.Message);
            Console.Write(ex.StackTrace);
            Console.Write($"[/{ECloudedFunctionBlock.CloudedFunctionError}]");
        }
    }

    private static TContext? ParseContext<TContext>(string[] args)
    {
        var context = JsonSerializer.Deserialize<TContext>(args[2]);

        if (context == null)
            throw new DataException("CONTEXT_MISSING");

        return context;
    }
}
