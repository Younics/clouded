using Microsoft.AspNetCore.Components;

namespace Clouded.Admin.Provider.Components;

public abstract class CancellableComponentBase : ComponentBase, IDisposable, IAsyncDisposable
{
    private readonly CancellationTokenSource _cts = new();
    protected CancellationToken CancellationToken => _cts.Token;

    protected virtual void DisposeComponent() { }

    protected virtual ValueTask DisposeComponentAsync() => ValueTask.CompletedTask;

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();

        DisposeComponent();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();
        await DisposeComponentAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
