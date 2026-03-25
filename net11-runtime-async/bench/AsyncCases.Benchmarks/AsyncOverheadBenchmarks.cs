using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace AsyncCases.Benchmarks;

[MemoryDiagnoser]
[Config(typeof(MultiTfmConfig))]
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
