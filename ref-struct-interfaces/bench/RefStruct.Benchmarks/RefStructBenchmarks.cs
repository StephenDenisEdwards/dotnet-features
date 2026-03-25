using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace RefStruct.Benchmarks;

[MemoryDiagnoser]
public class RefStructBenchmarks
{
    private const int Count = 1024;

    /// <summary>
    /// Generic Sum over a ReadOnlySpan (no heap allocation needed).
    /// </summary>
    private static T SumSpan<T>(ReadOnlySpan<T> values) where T : INumber<T>
    {
        T sum = T.Zero;
        foreach (var v in values)
            sum += v;
        return sum;
    }

    /// <summary>
    /// Generic Sum over an array (heap-allocated).
    /// </summary>
    private static T SumArray<T>(T[] values) where T : INumber<T>
    {
        T sum = T.Zero;
        foreach (var v in values)
            sum += v;
        return sum;
    }

    [Benchmark(Baseline = true)]
    public int SumWithSpan()
    {
        Span<int> data = stackalloc int[Count];
        for (int i = 0; i < Count; i++)
            data[i] = i;

        return SumSpan<int>(data);
    }

    [Benchmark]
    public int SumWithArray()
    {
        int[] data = new int[Count];
        for (int i = 0; i < Count; i++)
            data[i] = i;

        return SumArray(data);
    }
}
