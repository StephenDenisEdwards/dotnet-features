using BenchmarkDotNet.Attributes;

namespace AlternateLookup.Benchmarks;

[MemoryDiagnoser]
public class LookupBenchmarks
{
    [Params(100, 10_000)]
    public int DictionarySize { get; set; }

    private Dictionary<string, int> _dictionary = null!;
    private Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> _alternateLookup;
    private string _sourceString = null!;
    private string _lookupKey = null!;
    private int _keyStart;
    private int _keyLength;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _dictionary = new Dictionary<string, int>(DictionarySize);

        for (int i = 0; i < DictionarySize; i++)
        {
            _dictionary[$"key-{i:D6}"] = i;
        }

        // Pick a key that exists in the dictionary — somewhere in the middle
        int target = DictionarySize / 2;
        _lookupKey = $"key-{target:D6}";

        // Build a source string that contains the key embedded in other text
        _sourceString = $"prefix-{_lookupKey}-suffix";
        _keyStart = "prefix-".Length;
        _keyLength = _lookupKey.Length;

        _alternateLookup = _dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    [Benchmark(Baseline = true)]
    public bool StringLookup()
    {
        return _dictionary.TryGetValue(_lookupKey, out _);
    }

    [Benchmark]
    public bool SubstringLookup()
    {
        // Allocates a new string via Substring
        string substring = _sourceString.Substring(_keyStart, _keyLength);
        return _dictionary.TryGetValue(substring, out _);
    }

    [Benchmark]
    public bool SpanLookup()
    {
        // Zero-allocation span-based lookup
        ReadOnlySpan<char> span = _sourceString.AsSpan(_keyStart, _keyLength);
        return _alternateLookup.TryGetValue(span, out _);
    }
}
