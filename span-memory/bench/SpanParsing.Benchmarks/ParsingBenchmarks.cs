using BenchmarkDotNet.Attributes;

namespace SpanParsing.Benchmarks;

[MemoryDiagnoser]
public class ParsingBenchmarks
{
    private const string Input = "Temperature:42";
    private const int PrefixLength = 12; // "Temperature:".Length

    [Benchmark(Baseline = true)]
    public int SubstringParse()
    {
        string numberStr = Input.Substring(PrefixLength);
        return int.Parse(numberStr);
    }

    [Benchmark]
    public int SpanSliceParse()
    {
        ReadOnlySpan<char> span = Input.AsSpan();
        ReadOnlySpan<char> numberSpan = span[PrefixLength..];
        return int.Parse(numberSpan);
    }
}
