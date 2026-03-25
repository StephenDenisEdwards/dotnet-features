using System.Numerics;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace Simd.Benchmarks;

[MemoryDiagnoser]
public class ContainsBenchmarks
{
    [Params(256, 4096, 65536)]
    public int ArraySize { get; set; }

    private float[] _data = null!;
    private float _target;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _data = new float[ArraySize];
        var rng = new Random(42);
        for (int i = 0; i < _data.Length; i++)
            _data[i] = rng.NextSingle() * 10_000f;

        // Place the target near the end so we measure full-scan throughput
        _target = _data[^(ArraySize / 20)];
    }

    [Benchmark(Baseline = true)]
    public bool ScalarEarlyExit()
    {
        float[] data = _data;
        float target = _target;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == target)
                return true;
        }
        return false;
    }

    [Benchmark]
    public bool VectorT()
    {
        float[] data = _data;
        float target = _target;
        int vectorSize = Vector<float>.Count;
        var vTarget = new Vector<float>(target);
        int i = 0;

        for (; i <= data.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<float>(data, i);
            if (Vector.EqualsAny(v, vTarget))
                return true;
        }

        for (; i < data.Length; i++)
        {
            if (data[i] == target)
                return true;
        }

        return false;
    }

    [Benchmark]
    public bool Vector256T()
    {
        if (!Vector256.IsHardwareAccelerated)
            return ScalarEarlyExit();

        float[] data = _data;
        float target = _target;
        int vectorSize = Vector256<float>.Count;
        var vTarget = Vector256.Create(target);
        ReadOnlySpan<float> span = data;
        int i = 0;

        for (; i <= span.Length - vectorSize; i += vectorSize)
        {
            var v = Vector256.Create(span.Slice(i, vectorSize));
            var mask = Vector256.Equals(v, vTarget);
            if (mask != Vector256<float>.Zero)
                return true;
        }

        for (; i < data.Length; i++)
        {
            if (data[i] == target)
                return true;
        }

        return false;
    }

    [Benchmark]
    public bool LinqContains()
    {
        return _data.Contains(_target);
    }
}
