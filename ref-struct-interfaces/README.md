# ref struct Interfaces

Demonstrates the C# 13 feature that allows `ref struct` types to implement interfaces. This enables ref structs to be used with `IDisposable` (in `using` statements) and other interfaces — all while remaining stack-allocated.

The console app shows a `ref struct ResourceHandle` implementing `IDisposable` and a generic `Sum<T>` method constrained to `INumber<T>` that accepts `ReadOnlySpan<T>`. The benchmark project compares stack-allocated `Span<int>` (zero allocation) against heap-allocated `int[]` to highlight the allocation difference.

## Run the console app

```shell
dotnet run --project ref-struct-interfaces/console-app
```

## Run the benchmarks

```shell
dotnet run --project ref-struct-interfaces/bench/RefStruct.Benchmarks -c Release
```
