using System.Text;
using BenchmarkDotNet.Attributes;

namespace CompositeFormat.Benchmarks;

[MemoryDiagnoser]
public class FormattingBenchmarks
{
    private const string FormatString = "Name: {0}, Age: {1}, Score: {2:F2}";

    private static readonly System.Text.CompositeFormat ParsedFormat =
        System.Text.CompositeFormat.Parse(FormatString);

    private string _name = null!;
    private int _age;
    private double _score;
    private StringBuilder _sb = null!;

    [GlobalSetup]
    public void Setup()
    {
        _name = "Alice";
        _age = 30;
        _score = 97.856;
        _sb = new StringBuilder(128);
    }

    [Benchmark(Baseline = true)]
    public string StringFormat()
    {
        return string.Format(FormatString, _name, _age, _score);
    }

    [Benchmark]
    public string CompositeFormatParsed()
    {
        return string.Format(null, ParsedFormat, _name, _age, _score);
    }

    [Benchmark]
    public string StringInterpolation()
    {
        return $"Name: {_name}, Age: {_age}, Score: {_score:F2}";
    }

    [Benchmark]
    public string StringBuilderAppendFormat()
    {
        _sb.Clear();
        _sb.AppendFormat(null, ParsedFormat, _name, _age, _score);
        return _sb.ToString();
    }
}
