# dotnet-features

A collection of focused investigations into new and existing .NET and C# features. Each project contains samples, console apps, and/or benchmarks that make it easier to compare behavior, generated IL, and performance.

## Projects

| Project | Feature | Description |
|---|---|---|
| [net11-runtime-async](./net11-runtime-async/README.md) | Runtime Async (.NET 11) | Compares `net10.0` compiler-generated async state machines with `net11.0` `runtime-async=on`. Includes IL disassembly, BenchmarkDotNet benchmarks, and analysis of Preview 2 results. |
| [field-keyword](./field-keyword/README.md) | `field` keyword (C# 14) | Auto-property backing field access with validation — IL comparison showing identical output to explicit backing fields. |
| [null-conditional-assignment](./null-conditional-assignment/README.md) | Null-conditional assignment (C# 14) | `obj?.Prop = value` syntax — IL comparison against traditional null checks. |
| [extension-members](./extension-members/README.md) | Extension members (C# 14) | New `extension` blocks with extension properties vs traditional static extension methods. |
| [interceptors](./interceptors/README.md) | Interceptors | Compile-time method replacement via `[InterceptsLocation]` — IL comparison showing call site rewriting. |
| [system-threading-lock](./system-threading-lock/README.md) | `System.Threading.Lock` | Benchmarks the .NET 9+ `Lock` type against `Monitor`, `SemaphoreSlim`, and `SpinLock`. |
| [frozen-collections](./frozen-collections/README.md) | Frozen collections | `FrozenDictionary` vs `Dictionary` vs `ImmutableDictionary` — lookup and creation benchmarks. |
| [params-span](./params-span/README.md) | `params ReadOnlySpan<T>` (C# 13) | Allocation comparison: `params ReadOnlySpan<T>` (stack) vs `params T[]` (heap). IL comparison and benchmarks. |
| [search-values-string](./search-values-string/README.md) | `SearchValues<string>` (.NET 10) | SIMD multi-string search vs `Regex` vs manual `IndexOf` loop. |
| [task-when-each](./task-when-each/README.md) | `Task.WhenEach` (.NET 10) | Process tasks as they complete vs `Task.WhenAll` — ergonomics demo and benchmarks. |
| [ref-struct-interfaces](./ref-struct-interfaces/README.md) | ref struct interfaces (C# 13) | `ref struct` implementing `IDisposable` — benchmarks showing allocation savings with `Span<T>` in generic contexts. |
| [native-aot](./native-aot/README.md) | NativeAOT | JIT vs NativeAOT comparison — startup time, binary size, and working set. |
| [zip-archive-improvements](./zip-archive-improvements/README.md) | ZipArchive compression | Sequential vs parallel compression benchmarks. |
