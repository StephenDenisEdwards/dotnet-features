using System.Threading.RateLimiting;

// --- 1. FixedWindowRateLimiter ---
Console.WriteLine("=== FixedWindowRateLimiter ===");
Console.WriteLine("  PermitLimit=5, Window=1s");
using (var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
{
    PermitLimit = 5,
    Window = TimeSpan.FromSeconds(1),
    AutoReplenishment = false,
    QueueLimit = 0
}))
{
    int acquired = 0, rejected = 0;
    for (int i = 0; i < 10; i++)
    {
        using var lease = limiter.AttemptAcquire();
        if (lease.IsAcquired) acquired++; else rejected++;
    }
    Console.WriteLine($"  Acquired: {acquired}, Rejected: {rejected}");
}

// --- 2. SlidingWindowRateLimiter ---
Console.WriteLine();
Console.WriteLine("=== SlidingWindowRateLimiter ===");
Console.WriteLine("  PermitLimit=5, Window=1s, SegmentsPerWindow=2");
using (var limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
{
    PermitLimit = 5,
    Window = TimeSpan.FromSeconds(1),
    SegmentsPerWindow = 2,
    AutoReplenishment = false,
    QueueLimit = 0
}))
{
    int acquired = 0, rejected = 0;
    for (int i = 0; i < 10; i++)
    {
        using var lease = limiter.AttemptAcquire();
        if (lease.IsAcquired) acquired++; else rejected++;
    }
    Console.WriteLine($"  Acquired: {acquired}, Rejected: {rejected}");
}

// --- 3. TokenBucketRateLimiter ---
Console.WriteLine();
Console.WriteLine("=== TokenBucketRateLimiter ===");
Console.WriteLine("  TokenLimit=5, ReplenishmentPeriod=1s, TokensPerPeriod=2");
using (var limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
{
    TokenLimit = 5,
    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
    TokensPerPeriod = 2,
    AutoReplenishment = false,
    QueueLimit = 0
}))
{
    int acquired = 0, rejected = 0;
    for (int i = 0; i < 10; i++)
    {
        using var lease = limiter.AttemptAcquire();
        if (lease.IsAcquired) acquired++; else rejected++;
    }
    Console.WriteLine($"  Acquired: {acquired}, Rejected: {rejected}");
}

// --- 4. ConcurrencyLimiter ---
Console.WriteLine();
Console.WriteLine("=== ConcurrencyLimiter ===");
Console.WriteLine("  PermitLimit=2, simulating concurrent requests");
using (var limiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions
{
    PermitLimit = 2,
    QueueLimit = 0
}))
{
    // Hold two leases to fill the limit
    var lease1 = limiter.AttemptAcquire();
    var lease2 = limiter.AttemptAcquire();
    var lease3 = limiter.AttemptAcquire();
    Console.WriteLine($"  Lease 1 acquired: {lease1.IsAcquired}");
    Console.WriteLine($"  Lease 2 acquired: {lease2.IsAcquired}");
    Console.WriteLine($"  Lease 3 acquired (should fail): {lease3.IsAcquired}");

    // Release one and try again
    lease1.Dispose();
    var lease4 = limiter.AttemptAcquire();
    Console.WriteLine($"  Lease 4 acquired (after releasing 1): {lease4.IsAcquired}");

    lease2.Dispose();
    lease3.Dispose();
    lease4.Dispose();
}

Console.WriteLine();
Console.WriteLine("Done.");
