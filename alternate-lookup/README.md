# Alternate Lookup

`Dictionary<TKey,TValue>.GetAlternateLookup<TAlternate>()` (.NET 9+) enables dictionary lookups by `ReadOnlySpan<char>` without allocating a string. This is useful for hot-path parsing scenarios where keys arrive as spans — for example, when parsing CSV data, JSON tokens, or protocol buffers — and you want to avoid the allocation cost of converting each span to a string just to perform a lookup.

## Run

```bash
# Console app
dotnet run --project console-app/console-app.csproj

# Benchmarks
dotnet run --project bench/AlternateLookup.Benchmarks/AlternateLookup.Benchmarks.csproj -c Release
```
