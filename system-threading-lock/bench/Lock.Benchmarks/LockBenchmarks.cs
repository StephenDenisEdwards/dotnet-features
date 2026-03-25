using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class LockBenchmarks
{
    private readonly object _objLock = new();
    private readonly Lock _lock = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private SpinLock _spinLock = new(false);
    private int _counter;

    [Benchmark]
    public void MonitorLock()
    {
        lock (_objLock) { _counter++; }
    }

    [Benchmark]
    public void SystemLock()
    {
        using (_lock.EnterScope()) { _counter++; }
    }

    [Benchmark]
    public void SemaphoreSlim_Sync()
    {
        _semaphore.Wait();
        try { _counter++; }
        finally { _semaphore.Release(); }
    }

    [Benchmark]
    public void SpinLock_Lock()
    {
        bool taken = false;
        _spinLock.Enter(ref taken);
        try { _counter++; }
        finally { if (taken) _spinLock.Exit(); }
    }
}
