# dotnet-features

A collection of focused investigations into new and existing .NET and C# features. Each project contains samples, console apps, and/or benchmarks that make it easier to compare behavior, generated IL, and performance.

## Projects

| Project | Feature | Description |
|---|---|---|
| [net11-runtime-async](./net11-runtime-async/README.md) | Runtime Async (.NET 11) | Compares `net10.0` compiler-generated async state machines with `net11.0` `runtime-async=on`. Includes IL disassembly, BenchmarkDotNet benchmarks, and analysis of Preview 2 results. |
