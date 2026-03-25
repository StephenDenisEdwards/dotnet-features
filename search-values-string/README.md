# SearchValues&lt;string&gt; (.NET 10)

.NET 10 introduces `SearchValues<string>`, extending the existing `SearchValues<T>` type
to support efficient multi-string search within a `ReadOnlySpan<char>`.

## Key API

```csharp
// Create a precomputed searcher for a set of strings
SearchValues<string> sv = SearchValues.Create(
    ["GET", "POST", "PUT", "DELETE"], StringComparison.OrdinalIgnoreCase);

ReadOnlySpan<char> text = "Received a GET request";

bool found = text.ContainsAny(sv);   // true
int  index = text.IndexOfAny(sv);    // 12
```

The runtime builds an optimised automaton once at creation time, then each
`ContainsAny` / `IndexOfAny` call performs a single scan over the haystack
instead of looping through every candidate string.

## Benchmarks

The `bench/SearchValues.Benchmarks` project compares:

| Approach | Description |
|---|---|
| `SearchValues<string>` | New .NET 10 multi-string searcher |
| `Regex` (source-generated) | Alternation pattern with `IgnoreCase` |
| Manual loop | `string.IndexOf` per candidate |

Run with:

```bash
dotnet run -c Release --project bench/SearchValues.Benchmarks -- --filter '*'
```
