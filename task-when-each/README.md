# Task.WhenEach

Demonstrates `Task.WhenEach`, introduced in .NET 10, which returns an `IAsyncEnumerable<Task<T>>` that yields tasks in completion order. This contrasts with `Task.WhenAll`, which waits for every task to finish before returning all results at once.

`Task.WhenEach` is useful when you want to process results as they become available rather than waiting for the slowest task to complete.

## Console App

```bash
cd console-app
dotnet run
```

## Benchmarks

```bash
cd bench/WhenEach.Benchmarks
dotnet run -c Release -- --filter *WhenEach*
```
