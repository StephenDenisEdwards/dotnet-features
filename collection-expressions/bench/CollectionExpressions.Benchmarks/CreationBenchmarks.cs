using BenchmarkDotNet.Attributes;

namespace CollectionExpressions.Benchmarks;

[MemoryDiagnoser]
public class CreationBenchmarks
{
    [Benchmark(Baseline = true)]
    public int[] ExplicitArray()
    {
        return new int[] { 1, 2, 3, 4, 5 };
    }

    [Benchmark]
    public int[] CollectionExpr_Array()
    {
        int[] result = [1, 2, 3, 4, 5];
        return result;
    }

    [Benchmark]
    public List<int> CollectionExpr_List()
    {
        List<int> result = [1, 2, 3, 4, 5];
        return result;
    }

    [Benchmark]
    public int[] CollectionExpr_Span()
    {
        // We return the span's content as an array so BDN can consume it.
        // The benchmark measures the span creation path.
        Span<int> result = [1, 2, 3, 4, 5];
        return result.ToArray();
    }

    [Benchmark]
    public List<int> TraditionalListInit()
    {
        return new List<int> { 1, 2, 3, 4, 5 };
    }
}
