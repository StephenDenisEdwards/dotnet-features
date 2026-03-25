using System;
using System.Buffers;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace SearchValues.Benchmarks;

/// <summary>
/// Compares three approaches for finding HTTP method strings inside a text haystack:
///   1. SearchValues&lt;string&gt;  (new in .NET 10)
///   2. Compiled Regex alternation
///   3. Manual loop with IndexOf per candidate
/// </summary>
[MemoryDiagnoser]
public partial class MultiStringSearchBenchmarks
{
    private static readonly string[] HttpMethods =
        ["GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS"];

    private static readonly System.Buffers.SearchValues<string> SearchValuesInstance =
        System.Buffers.SearchValues.Create(HttpMethods, StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex(@"GET|POST|PUT|DELETE|PATCH|HEAD|OPTIONS", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex HttpMethodRegex();

    private string _haystack = null!;

    [Params(
        "The server received a GET request followed by a POST request.",
        "No matching tokens appear in this particular sentence at all.",
        "Mixed traffic: OPTIONS preflight, then DELETE /api/resource, and finally a PATCH update.")]
    public string Haystack
    {
        get => _haystack;
        set => _haystack = value;
    }

    // ─── SearchValues<string> ────────────────────────────────────────

    [Benchmark(Description = "SearchValues<string>.ContainsAny")]
    public bool SearchValues_ContainsAny()
    {
        return _haystack.AsSpan().ContainsAny(SearchValuesInstance);
    }

    [Benchmark(Description = "SearchValues<string>.IndexOfAny")]
    public int SearchValues_IndexOfAny()
    {
        return _haystack.AsSpan().IndexOfAny(SearchValuesInstance);
    }

    // ─── Regex ───────────────────────────────────────────────────────

    [Benchmark(Description = "Regex.IsMatch")]
    public bool Regex_IsMatch()
    {
        return HttpMethodRegex().IsMatch(_haystack);
    }

    [Benchmark(Description = "Regex.Match (index)")]
    public int Regex_MatchIndex()
    {
        var m = HttpMethodRegex().Match(_haystack);
        return m.Success ? m.Index : -1;
    }

    // ─── Manual loop ─────────────────────────────────────────────────

    [Benchmark(Description = "ManualLoop.Contains")]
    public bool ManualLoop_Contains()
    {
        foreach (var method in HttpMethods)
        {
            if (_haystack.Contains(method, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    [Benchmark(Description = "ManualLoop.IndexOf")]
    public int ManualLoop_IndexOf()
    {
        int best = int.MaxValue;
        foreach (var method in HttpMethods)
        {
            int idx = _haystack.IndexOf(method, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0 && idx < best)
                best = idx;
        }
        return best == int.MaxValue ? -1 : best;
    }
}
