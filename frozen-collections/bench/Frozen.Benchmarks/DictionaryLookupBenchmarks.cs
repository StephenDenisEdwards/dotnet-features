using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;

namespace Frozen.Benchmarks;

[MemoryDiagnoser]
public class DictionaryLookupBenchmarks
{
    [Params(10, 100, 10_000)]
    public int Size { get; set; }

    private Dictionary<string, int> _dictionary = null!;
    private FrozenDictionary<string, int> _frozenDictionary = null!;
    private ImmutableDictionary<string, int> _immutableDictionary = null!;
    private string _lookupKey = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var source = new Dictionary<string, int>(Size);
        for (int i = 0; i < Size; i++)
        {
            source[$"key_{i}"] = i;
        }

        _dictionary = new Dictionary<string, int>(source);
        _frozenDictionary = source.ToFrozenDictionary();
        _immutableDictionary = source.ToImmutableDictionary();
        _lookupKey = $"key_{Size / 2}";
    }

    [Benchmark]
    public bool Dictionary_TryGetValue()
    {
        _dictionary.TryGetValue(_lookupKey, out int value);
        return value > 0;
    }

    [Benchmark]
    public bool FrozenDictionary_TryGetValue()
    {
        _frozenDictionary.TryGetValue(_lookupKey, out int value);
        return value > 0;
    }

    [Benchmark]
    public bool ImmutableDictionary_TryGetValue()
    {
        _immutableDictionary.TryGetValue(_lookupKey, out int value);
        return value > 0;
    }

    [Benchmark]
    public bool Dictionary_ContainsKey()
    {
        return _dictionary.ContainsKey(_lookupKey);
    }

    [Benchmark]
    public bool FrozenDictionary_ContainsKey()
    {
        return _frozenDictionary.ContainsKey(_lookupKey);
    }

    [Benchmark]
    public bool ImmutableDictionary_ContainsKey()
    {
        return _immutableDictionary.ContainsKey(_lookupKey);
    }
}
