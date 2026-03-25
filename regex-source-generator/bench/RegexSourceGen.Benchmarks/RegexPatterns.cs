using System.Text.RegularExpressions;

namespace RegexSourceGen.Benchmarks;

partial class RegexPatterns
{
    [GeneratedRegex(@"^[\w.+-]+@[\w-]+\.[\w.]+$", RegexOptions.Compiled)]
    public static partial Regex EmailRegex();

    [GeneratedRegex(@"\b\d{1,3}(\.\d{1,3}){3}\b", RegexOptions.Compiled)]
    public static partial Regex IpAddressRegex();
}
