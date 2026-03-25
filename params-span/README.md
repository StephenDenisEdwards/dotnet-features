# params ReadOnlySpan

Demonstrates the `params ReadOnlySpan<T>` feature introduced in C# 13, which allows `params` parameters to accept `ReadOnlySpan<T>` instead of arrays. This eliminates heap allocations when passing inline arguments, as the compiler can stack-allocate the span instead of creating a temporary array.

The console app shows both `params int[]` and `params ReadOnlySpan<int>` side by side for IL comparison. The benchmark project measures the allocation difference using BenchmarkDotNet with `[MemoryDiagnoser]`.

## Run the console app

```shell
dotnet run --project params-span/console-app
```

## Run the benchmarks

```shell
dotnet run --project params-span/bench/ParamsSpan.Benchmarks -c Release
```
