namespace STranslate.Core;

public sealed class DebounceExecutor : IDisposable
{
    private readonly Lock _lock = new();
    private long _generation;
    private bool _disposed = false;

    /// <summary>
    /// 取消当前待执行的防抖任务
    /// </summary>
    public void Cancel()
    {
        lock (_lock)
        {
            _generation++;
        }
    }

    /// <summary>
    /// 同步动作防抖执行
    /// </summary>
    public void Execute(Action action, TimeSpan delay)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var generation = NextGeneration();
        _ = DelayAndRunAsync(generation, delay, action);
    }

    /// <summary>
    /// 异步动作防抖执行 (支持 async/await)
    /// </summary>
    public void ExecuteAsync(Func<Task> asyncAction, TimeSpan delay)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var generation = NextGeneration();
        _ = DelayAndRunAsync(generation, delay, asyncAction);
    }

    private long NextGeneration()
    {
        lock (_lock)
        {
            _generation++;
            return _generation;
        }
    }

    private bool IsLatestGeneration(long generation)
    {
        lock (_lock)
        {
            return !_disposed && generation == _generation;
        }
    }

    private async Task DelayAndRunAsync(long generation, TimeSpan delay, Action action)
    {
        await Task.Delay(delay).ConfigureAwait(false);

        if (!IsLatestGeneration(generation))
            return;

        action.Invoke();
    }

    private async Task DelayAndRunAsync(long generation, TimeSpan delay, Func<Task> asyncAction)
    {
        await Task.Delay(delay).ConfigureAwait(false);

        if (!IsLatestGeneration(generation))
            return;

        await asyncAction().ConfigureAwait(false);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;
            _generation++;
        }
    }
}
