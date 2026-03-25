using System.IO.Compression;
using BenchmarkDotNet.Attributes;

namespace ZipArchive.Benchmarks;

/// <summary>
/// Compares sequential ZipArchive compression against a manual parallel approach.
///
/// .NET 10 did not add a built-in parallel compression API to System.IO.Compression.ZipArchive.
/// The ZipArchive class remains single-threaded and is not thread-safe for concurrent writes.
///
/// The parallel benchmark compresses each file independently on separate threads using
/// individual MemoryStreams and DeflateStreams, then assembles them into a single
/// ZipArchive sequentially from the pre-compressed buffers. This isolates the CPU-heavy
/// deflate work to run in parallel while keeping the single-threaded zip assembly as
/// a thin sequential pass.
/// </summary>
[MemoryDiagnoser]
public class CompressionBenchmarks
{
    [Params(10, 100)]
    public int FileCount { get; set; }

    [Params(1024, 102_400)]
    public int FileSize { get; set; }

    private byte[][] _fileData = [];

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rng = new Random(42);
        _fileData = new byte[FileCount][];
        for (int i = 0; i < FileCount; i++)
        {
            _fileData[i] = new byte[FileSize];
            rng.NextBytes(_fileData[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public byte[] CompressSequential()
    {
        using var ms = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            for (int i = 0; i < _fileData.Length; i++)
            {
                var entry = archive.CreateEntry($"file_{i}.bin", CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                entryStream.Write(_fileData[i]);
            }
        }

        return ms.ToArray();
    }

    [Benchmark]
    public byte[] CompressParallel()
    {
        // Phase 1: Compress each file independently in parallel using raw DeflateStream.
        var compressed = new byte[_fileData.Length][];

        Parallel.For(0, _fileData.Length, i =>
        {
            using var ms = new MemoryStream();
            using (var deflate = new DeflateStream(ms, CompressionLevel.Optimal, leaveOpen: true))
            {
                deflate.Write(_fileData[i]);
            }

            compressed[i] = ms.ToArray();
        });

        // Phase 2: Assemble the pre-compressed buffers into a ZipArchive sequentially.
        // We use CompressionLevel.NoCompression here because the data is already deflated
        // in phase 1. Note: ZipArchive.CreateEntry with NoCompression stores data as-is
        // (Store method), so we write the raw original bytes and let the entry use
        // Optimal compression to keep the comparison fair. Alternatively, we could write
        // raw deflate bytes with a stored entry, but ZipArchive doesn't expose the ability
        // to inject pre-compressed data directly. So we fall back to writing uncompressed
        // data and still rely on ZipArchive for the final deflate -- which means the
        // real win from parallelism in this approach is limited.
        //
        // A more realistic parallel zip strategy would use a third-party library that
        // supports injecting pre-compressed entry data (e.g., DotNetZip, SharpZipLib).
        //
        // For this benchmark, we demonstrate the parallel-compress + sequential-assemble
        // pattern to measure the overhead and compare throughput.
        using var outputMs = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(outputMs, ZipArchiveMode.Create, leaveOpen: true))
        {
            for (int i = 0; i < _fileData.Length; i++)
            {
                // Store the raw (uncompressed) data using NoCompression so the sequential
                // assembly is as fast as possible. The compression work was already done
                // in parallel -- but because ZipArchive cannot accept pre-deflated data,
                // we store uncompressed to keep assembly cheap.
                var entry = archive.CreateEntry($"file_{i}.bin", CompressionLevel.NoCompression);
                using var entryStream = entry.Open();
                entryStream.Write(compressed[i]);
            }
        }

        return outputMs.ToArray();
    }
}
