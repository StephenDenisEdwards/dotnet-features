# Span<T> and Memory<T>

Stack vs heap slicing, parsing without allocations, and `stackalloc` with spans. Compares against `string.Substring` and array copies.

`Span<T>` is a stack-only ref struct that provides a type-safe, memory-safe view over contiguous memory — arrays, stack-allocated buffers, or native memory. Because it lives on the stack, it avoids heap allocations for slicing and parsing operations that would otherwise require `string.Substring` or `Array.Copy`.

`Memory<T>` is the heap-friendly counterpart: it can be stored in fields and passed to async methods, making it suitable for scenarios where `Span<T>` cannot be used.

## Run the console app

```bash
cd span-memory/console-app
dotnet run
```

## Run the benchmarks

```bash
cd span-memory/bench/SpanParsing.Benchmarks
dotnet run -c Release
```
