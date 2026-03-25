using BenchmarkDotNet.Attributes;

namespace ParamsSpan.Benchmarks;

[MemoryDiagnoser]
public class ParamsAllocationBenchmarks
{
    [Benchmark]
    public int ParamsArray_3Args() => SumArray(1, 2, 3);

    [Benchmark]
    public int ParamsSpan_3Args() => SumSpan(1, 2, 3);

    [Benchmark]
    public int ParamsArray_10Args() => SumArray(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

    [Benchmark]
    public int ParamsSpan_10Args() => SumSpan(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

    static int SumArray(params int[] values)
    {
        int sum = 0;
        foreach (var v in values) sum += v;
        return sum;
    }

    static int SumSpan(params ReadOnlySpan<int> values)
    {
        int sum = 0;
        foreach (var v in values) sum += v;
        return sum;
    }
}
