using BenchmarkDotNet.Attributes;

namespace WhenEach.Benchmarks;

[MemoryDiagnoser]
public class WhenEachBenchmarks
{
    [Params(5, 50)]
    public int TaskCount { get; set; }

    private List<Task<int>> CreateTasks()
    {
        return Enumerable.Range(1, TaskCount)
            .Select(async i =>
            {
                await Task.Delay(1);
                return i;
            })
            .ToList();
    }

    [Benchmark(Baseline = true)]
    public async Task<int[]> WhenAll_ThenProcess()
    {
        var tasks = CreateTasks();
        var results = await Task.WhenAll(tasks);

        var sum = 0;
        foreach (var result in results)
        {
            sum += result;
        }

        return results;
    }

    [Benchmark]
    public async Task<List<int>> WhenEach_ProcessAsCompleted()
    {
        var tasks = CreateTasks();
        var results = new List<int>(TaskCount);

        await foreach (var task in Task.WhenEach(tasks))
        {
            results.Add(task.Result);
        }

        return results;
    }
}
