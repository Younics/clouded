using Clouded.Function.Framework.Contexts.Base;

namespace Clouded.Function.Framework.Components.Base;

public abstract class Hook<TContext, TOutput>
    where TContext : HookContext
{
    public abstract Task<TOutput> RunAsync(TContext context);
}
