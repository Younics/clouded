using System.Text.Json;
using Clouded.Function.Framework.Components.Base;
using Clouded.Function.Framework.Contexts.Base;

namespace Clouded.Function.Framework.Components;

public abstract class InputHook : Hook<HookContext, JsonElement> { }
