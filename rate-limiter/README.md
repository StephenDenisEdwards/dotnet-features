# rate-limiter

Demonstrates the built-in `System.Threading.RateLimiting` APIs (.NET 7+). Shows FixedWindowRateLimiter, SlidingWindowRateLimiter, TokenBucketRateLimiter, and ConcurrencyLimiter with benchmarks.

## Run

```bash
# Console app
dotnet run --project console-app/console-app.csproj

# Benchmarks
dotnet run --project bench/RateLimiter.Benchmarks/RateLimiter.Benchmarks.csproj -c Release
```
