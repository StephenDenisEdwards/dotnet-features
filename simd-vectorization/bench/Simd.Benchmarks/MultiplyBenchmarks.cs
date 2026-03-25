using System.Numerics;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace Simd.Benchmarks;

[MemoryDiagnoser]
public class MultiplyBenchmarks
{
    [Params(256, 4096, 65536)]
    public int ArraySize { get; set; }

    private float[] _source = null!;
    private float[] _destination = null!;
    private const float Multiplier = 2.5f;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _source = new float[ArraySize];
        _destination = new float[ArraySize];
        var rng = new Random(42);
        for (int i = 0; i < _source.Length; i++)
            _source[i] = rng.NextSingle() * 100f;
    }

    [Benchmark(Baseline = true)]
    public void Scalar()
    {
        float[] src = _source;
        float[] dst = _destination;
        for (int i = 0; i < src.Length; i++)
            dst[i] = src[i] * Multiplier;
    }

    [Benchmark]
    public void VectorT()
    {
        float[] src = _source;
        float[] dst = _destination;
        int vectorSize = Vector<float>.Count;
        var vMultiplier = new Vector<float>(Multiplier);
        int i = 0;

        for (; i <= src.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<float>(src, i);
            (v * vMultiplier).CopyTo(dst, i);
        }

        for (; i < src.Length; i++)
            dst[i] = src[i] * Multiplier;
    }

    [Benchmark]
    public void Vector256T()
    {
        if (!Vector256.IsHardwareAccelerated)
        {
            Scalar();
            return;
        }

        float[] src = _source;
        float[] dst = _destination;
        int vectorSize = Vector256<float>.Count;
        var vMultiplier = Vector256.Create(Multiplier);
        ReadOnlySpan<float> srcSpan = src;
        Span<float> dstSpan = dst;
        int i = 0;

        for (; i <= src.Length - vectorSize; i += vectorSize)
        {
            var v = Vector256.Create(srcSpan.Slice(i, vectorSize));
            var result = Vector256.Multiply(v, vMultiplier);
            result.CopyTo(dstSpan.Slice(i, vectorSize));
        }

        for (; i < src.Length; i++)
            dst[i] = src[i] * Multiplier;
    }

    [Benchmark]
    public void LinqSelect()
    {
        float[] result = _source.Select(x => x * Multiplier).ToArray();
    }
}
