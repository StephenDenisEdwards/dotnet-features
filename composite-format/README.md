# CompositeFormat

Pre-parsed format strings for `string.Format` and `StringBuilder.AppendFormat`, introduced in .NET 8.

`CompositeFormat.Parse` parses a composite format string once and caches the result, eliminating repeated parsing overhead when the same format string is used in a hot loop. This benchmarks `CompositeFormat` against raw `string.Format`, string interpolation, and `StringBuilder.AppendFormat`.

## Run the benchmarks

```bash
cd composite-format/bench/CompositeFormat.Benchmarks
dotnet run -c Release
```
