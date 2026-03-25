# Regex Source Generator

The `[GeneratedRegex]` attribute, introduced in .NET 7 (C# 12+), invokes a Roslyn source generator at compile time to emit an optimised, allocation-free regex engine tailored to the specific pattern. Instead of interpreting or JIT-compiling the pattern at runtime, the generated code is a direct state-machine implementation that the JIT can further inline and optimise.

This project benchmarks four approaches to regex matching:

1. **Source-generated** (`[GeneratedRegex]`) - compile-time optimised code.
2. **Runtime-compiled** (`new Regex(pattern, RegexOptions.Compiled)`) - JIT-compiled at first use.
3. **Interpreted** (`new Regex(pattern)`) - no compilation, interpreted on every match.
4. **Static `Regex.IsMatch`** - uses the internal regex cache; effectively interpreted with caching.

Patterns tested: email validation and IPv4 address detection, each exercised against a mix of matching and non-matching inputs.

## Run

```bash
cd regex-source-generator/bench/RegexSourceGen.Benchmarks
dotnet run -c Release -- --filter *
```
