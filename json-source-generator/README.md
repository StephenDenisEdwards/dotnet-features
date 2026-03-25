# JSON Source Generator

`[JsonSerializable]` source-generated JSON serialization (.NET 7+ / System.Text.Json). Pre-generates serialization code at compile time, eliminating reflection overhead. This makes serialization faster, reduces memory allocations, and is fully NativeAOT compatible.

## Console App

```bash
cd console-app
dotnet run
```

## Benchmarks

```bash
cd bench/JsonSourceGen.Benchmarks
dotnet run -c Release
```
