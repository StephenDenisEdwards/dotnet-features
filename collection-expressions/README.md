# Collection Expressions

Collection expressions (C# 12+) introduce the `[1, 2, 3]` syntax for creating collections, along with the spread operator `[..existing, newItem]`. The compiler chooses different backing storage strategies depending on the target type — arrays, lists, spans, and immutable arrays all get optimized differently.

## Key Concepts

- **Target-typed creation:** The same `[1, 2, 3]` syntax produces different IL depending on whether the target is `int[]`, `List<int>`, `Span<int>`, `ReadOnlySpan<int>`, or `ImmutableArray<int>`.
- **Spread operator:** `[..a, ..b]` concatenates collections efficiently without intermediate allocations (in some cases).
- **Compiler optimizations:** `ReadOnlySpan<int>` of constants can be backed by static data in the assembly, avoiding heap allocation entirely.

## Run

```bash
# Console demo
cd collection-expressions/console-app
dotnet run

# Benchmarks
cd collection-expressions/bench/CollectionExpressions.Benchmarks
dotnet run -c Release -- --filter *
```

## IL Analysis

```bash
cd collection-expressions/console-app
dotnet build -c Release
# Use ILSpy, ildasm, or sharplab.io to inspect the output assembly
```
