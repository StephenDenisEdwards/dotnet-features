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
| [source-generators](./source-generators/README.md) | Source Generators | Incremental source generator that auto-generates `ToString()` overrides from `[GenerateToString]`-annotated classes. |
| [span-memory](./span-memory/README.md) | `Span<T>` and `Memory<T>` | Stack vs heap slicing, parsing without allocations, `stackalloc` with spans — benchmarks against `Substring` and `Array.Copy`. |
| [composite-format](./composite-format/README.md) | `CompositeFormat` (.NET 8+) | Pre-parsed format strings for `string.Format` — benchmarks vs interpolation and `StringBuilder`. |
| [alternate-lookup](./alternate-lookup/README.md) | `AlternateLookup` (.NET 9+) | Dictionary lookups by `ReadOnlySpan<char>` without allocating a string key. |
| [simd-vectorization](./simd-vectorization/README.md) | SIMD / `Vector<T>` | Manual vectorization with `Vector<T>` and `Vector256<T>` vs scalar loops and LINQ. |
| [primary-constructors](./primary-constructors/README.md) | Primary constructors (C# 12+) | Capture semantics, mutable parameter gotcha, and IL comparison to traditional constructors. |
| [collection-expressions](./collection-expressions/README.md) | Collection expressions (C# 12+) | `[1, 2, 3]` syntax, spread `[..a, ..b]`, and compiler backing-storage choices — IL analysis and benchmarks. |
| [allows-ref-struct](./allows-ref-struct/README.md) | `allows ref struct` (C# 13+) | Generic type parameters accepting `Span<T>` — interface implementation and IL behavior. |
| [regex-source-generator](./regex-source-generator/README.md) | `[GeneratedRegex]` | Source-generated regex vs runtime-compiled and interpreted — throughput benchmarks. |
| [json-source-generator](./json-source-generator/README.md) | `[JsonSerializable]` | Source-generated `System.Text.Json` serialization vs reflection — benchmarks for serialize and deserialize. |
| [time-provider](./time-provider/README.md) | `TimeProvider` (.NET 8+) | Testable time abstraction — `FakeTimeProvider` for deterministic testing vs `TimeProvider.System`. |
| [async-enumerable-channels](./async-enumerable-channels/README.md) | `IAsyncEnumerable<T>` and Channels | Producer/consumer streaming with `Channel<T>` vs `BlockingCollection<T>` vs `IAsyncEnumerable`. |
| [rate-limiter](./rate-limiter/README.md) | `RateLimiting` (.NET 7+) | Built-in rate limiters — fixed window, sliding window, token bucket, and concurrency limiter demos and benchmarks. |
| [distributed-tracing](./distributed-tracing/README.md) | `ActivitySource` / distributed tracing | BCL-native OpenTelemetry-compatible tracing — spans, tags, events, baggage, and parent-child relationships. |
