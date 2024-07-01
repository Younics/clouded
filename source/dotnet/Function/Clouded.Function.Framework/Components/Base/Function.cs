using System.Text.Json;
using Clouded.Function.Framework.Contexts;

namespace Clouded.Function.Framework.Components.Base;

public abstract class Function : Hook<FunctionContext, JsonElement> { }
