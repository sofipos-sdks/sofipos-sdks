namespace SofiPOS.FnB.Demo.Services;

// Một phiên kết nối cho backend demo; đổi kết nối chờ lời gọi hiện hành kết thúc.
public sealed class WorkspaceSession
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly AsyncLocal<bool> _inside = new();

    public async Task<T> RunAsync<T>(Func<Task<T>> action, CancellationToken ct = default)
    {
        if (_inside.Value) return await action();
        await _gate.WaitAsync(ct);
        _inside.Value = true;
        try { return await action(); }
        finally { _inside.Value = false; _gate.Release(); }
    }

    public Task RunAsync(Func<Task> action, CancellationToken ct = default)
        => RunAsync(async () => { await action(); return true; }, ct);
}
