using System.Threading.RateLimiting;
using BenchmarkDotNet.Attributes;

namespace RateLimiter.Benchmarks;

[MemoryDiagnoser]
public class AcquireBenchmarks
{
    private FixedWindowRateLimiter _fixedWindow = null!;
    private SlidingWindowRateLimiter _slidingWindow = null!;
    private TokenBucketRateLimiter _tokenBucket = null!;
    private ConcurrencyLimiter _concurrency = null!;

    [GlobalSetup]
    public void Setup()
    {
        _fixedWindow = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = int.MaxValue,
            Window = TimeSpan.FromHours(1),
            AutoReplenishment = false,
            QueueLimit = 0
        });

        _slidingWindow = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = int.MaxValue,
            Window = TimeSpan.FromHours(1),
            SegmentsPerWindow = 1,
            AutoReplenishment = false,
            QueueLimit = 0
        });

        _tokenBucket = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = int.MaxValue,
            ReplenishmentPeriod = TimeSpan.FromHours(1),
            TokensPerPeriod = int.MaxValue,
            AutoReplenishment = false,
            QueueLimit = 0
        });

        _concurrency = new ConcurrencyLimiter(new ConcurrencyLimiterOptions
        {
            PermitLimit = int.MaxValue,
            QueueLimit = 0
        });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _fixedWindow.Dispose();
        _slidingWindow.Dispose();
        _tokenBucket.Dispose();
        _concurrency.Dispose();
    }

    [Benchmark]
    public void FixedWindow()
    {
        using var lease = _fixedWindow.AttemptAcquire();
    }

    [Benchmark]
    public void SlidingWindow()
    {
        using var lease = _slidingWindow.AttemptAcquire();
    }

    [Benchmark]
    public void TokenBucket()
    {
        using var lease = _tokenBucket.AttemptAcquire();
    }

    [Benchmark]
    public void Concurrency()
    {
        using var lease = _concurrency.AttemptAcquire();
    }
}
