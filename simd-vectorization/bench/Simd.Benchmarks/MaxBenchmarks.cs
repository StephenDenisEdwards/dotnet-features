using System.Numerics;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace Simd.Benchmarks;

[MemoryDiagnoser]
public class MaxBenchmarks
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
            _data[i] = rng.NextSingle() * 10_000f;
    }

    [Benchmark(Baseline = true)]
    public float Scalar()
    {
        float[] data = _data;
        float max = float.MinValue;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > max)
                max = data[i];
        }
        return max;
    }

    [Benchmark]
    public float VectorT()
    {
        float[] data = _data;
        int vectorSize = Vector<float>.Count;
        var vMax = new Vector<float>(float.MinValue);
        int i = 0;

        for (; i <= data.Length - vectorSize; i += vectorSize)
            vMax = Vector.Max(vMax, new Vector<float>(data, i));

        // Horizontal reduction — extract the max from all lanes
        float max = float.MinValue;
        for (int lane = 0; lane < vectorSize; lane++)
        {
            if (vMax[lane] > max)
                max = vMax[lane];
        }

        // Handle remaining elements
        for (; i < data.Length; i++)
        {
            if (data[i] > max)
                max = data[i];
        }

        return max;
    }

    [Benchmark]
    public float Vector256T()
    {
        if (!Vector256.IsHardwareAccelerated)
            return Scalar();

        float[] data = _data;
        int vectorSize = Vector256<float>.Count;
        var vMax = Vector256.Create(float.MinValue);
        ReadOnlySpan<float> span = data;
        int i = 0;

        for (; i <= span.Length - vectorSize; i += vectorSize)
            vMax = Vector256.Max(vMax, Vector256.Create(span.Slice(i, vectorSize)));

        // Horizontal reduction
        float max = float.MinValue;
        for (int lane = 0; lane < vectorSize; lane++)
        {
            if (vMax[lane] > max)
                max = vMax[lane];
        }

        for (; i < data.Length; i++)
        {
            if (data[i] > max)
                max = data[i];
        }

        return max;
    }

    [Benchmark]
    public float LinqMax()
    {
        return _data.Max();
    }
}
