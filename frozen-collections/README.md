# Frozen Collections

Benchmarks comparing `FrozenDictionary<TKey, TValue>` (from `System.Collections.Frozen`) against `Dictionary<TKey, TValue>` and `ImmutableDictionary<TKey, TValue>` for both lookup and creation scenarios.

## Run

```bash
dotnet run -c Release --project frozen-collections/bench/Frozen.Benchmarks -- --filter "*"
```
