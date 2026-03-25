using BenchmarkDotNet.Attributes;

namespace SpanParsing.Benchmarks;

[MemoryDiagnoser]
public class SlicingBenchmarks
{
    [Params(100, 10_000)]
    public int Size;

    private int[] _source = null!;
    private const int SliceOffset = 10;
    private const int SliceLength = 50;

    [GlobalSetup]
    public void Setup()
    {
        _source = Enumerable.Range(0, Size).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] ArrayCopy()
    {
        int[] dest = new int[SliceLength];
        Array.Copy(_source, SliceOffset, dest, 0, SliceLength);
        return dest;
    }

    [Benchmark]
    public int SpanSlice()
    {
        Span<int> slice = _source.AsSpan(SliceOffset, SliceLength);
        int sum = 0;
        for (int i = 0; i < slice.Length; i++)
            sum += slice[i];
        return sum;
    }

    [Benchmark]
    public int ArraySegment()
    {
        var segment = new ArraySegment<int>(_source, SliceOffset, SliceLength);
        int sum = 0;
        foreach (int item in segment)
            sum += item;
        return sum;
    }
}
