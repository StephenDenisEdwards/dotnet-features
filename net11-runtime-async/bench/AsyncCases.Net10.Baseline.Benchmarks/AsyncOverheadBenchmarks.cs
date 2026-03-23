using AsyncCases;
using BenchmarkDotNet.Attributes;

namespace AsyncCases.Benchmarks;

[MemoryDiagnoser]
[InProcess]
public class AsyncOverheadBenchmarks
{
    [Benchmark(Baseline = true)]
    public int SynchronousBaseline() => AsyncWorkloads.SynchronousBaseline();

    [Benchmark]
    public Task CompletedTask() => AsyncWorkloads.CompletedTask();

    [Benchmark]
    public ValueTask CompletedValueTask() => AsyncWorkloads.CompletedValueTask();

    [Benchmark]
    public Task AwaitCompletedTask() => AsyncWorkloads.AwaitCompletedTask();

    [Benchmark]
    public ValueTask AwaitCompletedValueTask() => AsyncWorkloads.AwaitCompletedValueTask();

    [Benchmark]
    public Task<int> AwaitFromResult() => AsyncWorkloads.AwaitFromResult();

    [Benchmark]
    public Task<int> AwaitYield() => AsyncWorkloads.AwaitYield();
}
