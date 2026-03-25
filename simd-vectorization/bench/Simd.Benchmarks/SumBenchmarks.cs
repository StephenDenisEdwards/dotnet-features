using System.Numerics;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace Simd.Benchmarks;

[MemoryDiagnoser]
public class SumBenchmarks
{
    [Params(256, 4096, 65536)]
    public int ArraySize { get; set; }

    private float[] _data = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _data = new float[ArraySize];
        var rng = new Random(42);
        for (int i = 0; i < _data.Length; i++)
            _data[i] = rng.NextSingle() * 100f;
    }

    [Benchmark(Baseline = true)]
    public float ScalarSum()
    {
        float sum = 0f;
        float[] data = _data;
        for (int i = 0; i < data.Length; i++)
            sum += data[i];
        return sum;
    }

    [Benchmark]
    public float VectorSum()
    {
        float[] data = _data;
        int vectorSize = Vector<float>.Count;
        int i = 0;
        var vSum = Vector<float>.Zero;

        for (; i <= data.Length - vectorSize; i += vectorSize)
            vSum += new Vector<float>(data, i);

        float sum = Vector.Dot(vSum, Vector<float>.One);

        for (; i < data.Length; i++)
            sum += data[i];

        return sum;
    }

    [Benchmark]
    public float Vector256Sum()
    {
        if (!Vector256.IsHardwareAccelerated)
            return ScalarSum();

        float[] data = _data;
        int vectorSize = Vector256<float>.Count;
        int i = 0;
        var vSum = Vector256<float>.Zero;

        ReadOnlySpan<float> span = data;
        for (; i <= span.Length - vectorSize; i += vectorSize)
            vSum = Vector256.Add(vSum, Vector256.Create(span.Slice(i, vectorSize)));

        float sum = Vector256.Sum(vSum);

        for (; i < data.Length; i++)
            sum += data[i];

        return sum;
    }

    [Benchmark]
    public float LinqSum()
    {
        return _data.Sum();
    }
}
