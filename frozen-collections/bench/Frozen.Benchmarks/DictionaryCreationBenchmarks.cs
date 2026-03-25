using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;

namespace Frozen.Benchmarks;

[MemoryDiagnoser]
public class DictionaryCreationBenchmarks
{
    [Params(10, 100, 10_000)]
    public int Size { get; set; }

    private KeyValuePair<string, int>[] _source = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _source = new KeyValuePair<string, int>[Size];
        for (int i = 0; i < Size; i++)
        {
            _source[i] = new KeyValuePair<string, int>($"key_{i}", i);
        }
    }

    [Benchmark]
    public Dictionary<string, int> CreateDictionary()
    {
        return new Dictionary<string, int>(_source);
    }

    [Benchmark]
    public FrozenDictionary<string, int> CreateFrozenDictionary()
    {
        return _source.ToFrozenDictionary();
    }

    [Benchmark]
    public ImmutableDictionary<string, int> CreateImmutableDictionary()
    {
        return _source.ToImmutableDictionary();
    }
}
