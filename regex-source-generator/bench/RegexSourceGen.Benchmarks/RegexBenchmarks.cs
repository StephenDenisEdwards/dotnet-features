using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace RegexSourceGen.Benchmarks;

[MemoryDiagnoser]
public class RegexBenchmarks
{
    private const string EmailPattern = @"^[\w.+-]+@[\w-]+\.[\w.]+$";
    private const string IpPattern = @"\b\d{1,3}(\.\d{1,3}){3}\b";

    private string[] _emailInputs = null!;
    private string[] _ipInputs = null!;

    private Regex _emailCompiled = null!;
    private Regex _emailInterpreted = null!;
    private Regex _ipCompiled = null!;
    private Regex _ipInterpreted = null!;

    [GlobalSetup]
    public void Setup()
    {
        _emailInputs =
        [
            "user@example.com",
            "not-an-email",
            "alice.bob+tag@sub.domain.org",
            "missing@dot",
            "a@b.c",
            "@no-local.com",
            "valid.one@test.io",
            "spaces in@address.com",
            "dash-ok@my-host.net",
            "UPPER@CASE.COM",
        ];

        _ipInputs =
        [
            "192.168.1.1",
            "not-an-ip",
            "10.0.0.255",
            "999.999.999.999",
            "172.16.0.1",
            "abc.def.ghi.jkl",
            "0.0.0.0",
            "255.255.255.255",
            "1234.1.1.1",
            "8.8.8.8",
        ];

        _emailCompiled = new Regex(EmailPattern, RegexOptions.Compiled);
        _emailInterpreted = new Regex(EmailPattern);
        _ipCompiled = new Regex(IpPattern, RegexOptions.Compiled);
        _ipInterpreted = new Regex(IpPattern);
    }

    // -----------------------------------------------------------------------
    // Email benchmarks
    // -----------------------------------------------------------------------

    [Benchmark(Description = "Email: SourceGenerated")]
    public int Email_SourceGenerated()
    {
        int count = 0;
        Regex regex = RegexPatterns.EmailRegex();
        foreach (string input in _emailInputs)
            if (regex.IsMatch(input))
                count++;
        return count;
    }

    [Benchmark(Description = "Email: Compiled")]
    public int Email_Compiled()
    {
        int count = 0;
        foreach (string input in _emailInputs)
            if (_emailCompiled.IsMatch(input))
                count++;
        return count;
    }

    [Benchmark(Description = "Email: Interpreted")]
    public int Email_Interpreted()
    {
        int count = 0;
        foreach (string input in _emailInputs)
            if (_emailInterpreted.IsMatch(input))
                count++;
        return count;
    }

    [Benchmark(Description = "Email: StaticIsMatch")]
    public int Email_StaticIsMatch()
    {
        int count = 0;
        foreach (string input in _emailInputs)
            if (Regex.IsMatch(input, EmailPattern))
                count++;
        return count;
    }

    // -----------------------------------------------------------------------
    // IP address benchmarks
    // -----------------------------------------------------------------------

    [Benchmark(Description = "IP: SourceGenerated")]
    public int Ip_SourceGenerated()
    {
        int count = 0;
        Regex regex = RegexPatterns.IpAddressRegex();
        foreach (string input in _ipInputs)
            if (regex.IsMatch(input))
                count++;
        return count;
    }

    [Benchmark(Description = "IP: Compiled")]
    public int Ip_Compiled()
    {
        int count = 0;
        foreach (string input in _ipInputs)
            if (_ipCompiled.IsMatch(input))
                count++;
        return count;
    }

    [Benchmark(Description = "IP: Interpreted")]
    public int Ip_Interpreted()
    {
        int count = 0;
        foreach (string input in _ipInputs)
            if (_ipInterpreted.IsMatch(input))
                count++;
        return count;
    }

    [Benchmark(Description = "IP: StaticIsMatch")]
    public int Ip_StaticIsMatch()
    {
        int count = 0;
        foreach (string input in _ipInputs)
            if (Regex.IsMatch(input, IpPattern))
                count++;
        return count;
    }
}
