# ZipArchive Compression Benchmarks

Benchmarks comparing sequential vs. parallel compression strategies with `System.IO.Compression.ZipArchive` on .NET 10.

## Background

.NET 10 did not introduce a built-in parallel compression API for `ZipArchive`. The class remains single-threaded and is not thread-safe for concurrent writes. This project benchmarks a manual parallel approach (compressing files independently via `DeflateStream` on multiple threads, then assembling them into a zip) against the standard sequential `ZipArchive` workflow.

## Benchmarks

| Benchmark | Description |
|---|---|
| `CompressSequential` | Creates a `ZipArchive` and adds entries one at a time (baseline) |
| `CompressParallel` | Compresses each file in parallel using `Parallel.For` + `DeflateStream`, then assembles into a `ZipArchive` with `NoCompression` to keep assembly cheap |

Parameters: `FileCount` (10, 100) and `FileSize` (1 KB, 100 KB).

## Run

```bash
cd zip-archive-improvements/bench/ZipArchive.Benchmarks
dotnet run -c Release -- --filter *CompressionBenchmarks*
```
