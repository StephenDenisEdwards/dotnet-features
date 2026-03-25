using BenchmarkDotNet.Attributes;

namespace CollectionExpressions.Benchmarks;

[MemoryDiagnoser]
public class SpreadBenchmarks
{
    [Params(10, 100, 1000)]
    public int N;

    private int[] _arr1 = null!;
    private int[] _arr2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _arr1 = Enumerable.Range(0, N).ToArray();
        _arr2 = Enumerable.Range(N, N).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] SpreadOperator()
    {
        int[] result = [.._arr1, .._arr2];
        return result;
    }

    [Benchmark]
    public int[] ManualArrayCopy()
    {
        var result = new int[_arr1.Length + _arr2.Length];
        Array.Copy(_arr1, 0, result, 0, _arr1.Length);
        Array.Copy(_arr2, 0, result, _arr1.Length, _arr2.Length);
        return result;
    }

    [Benchmark]
    public int[] LinqConcat()
    {
        return _arr1.Concat(_arr2).ToArray();
    }
}
