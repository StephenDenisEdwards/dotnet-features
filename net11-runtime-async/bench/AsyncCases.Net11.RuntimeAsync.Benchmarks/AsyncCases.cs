namespace AsyncCases;

public static class AsyncWorkloads
{
    public static int SynchronousBaseline() => 42;

    public static Task CompletedTask() => Task.CompletedTask;

    public static ValueTask CompletedValueTask() => ValueTask.CompletedTask;

    public static async Task AwaitCompletedTask()
    {
        await Task.CompletedTask;
    }

    public static async ValueTask AwaitCompletedValueTask()
    {
        await ValueTask.CompletedTask;
    }

    public static async Task<int> AwaitFromResult()
    {
        return await Task.FromResult(42);
    }

    public static async Task<int> AwaitYield()
    {
        await Task.Yield();
        return 42;
    }
}
