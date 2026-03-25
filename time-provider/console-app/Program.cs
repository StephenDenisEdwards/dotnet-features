Console.WriteLine("=== TimeProvider Abstraction Demo ===");
Console.WriteLine();

// --- Production mode: TimeProvider.System -----------------------------------

Console.WriteLine("--- Production mode (TimeProvider.System) ---");

var systemProvider = TimeProvider.System;
var productionEntry = new CacheEntry<string>("cached-value", systemProvider, TimeSpan.FromMinutes(5));

Console.WriteLine($"Current UTC time : {systemProvider.GetUtcNow():O}");
Console.WriteLine($"Cache value      : {productionEntry.Value}");
Console.WriteLine($"Is expired?      : {productionEntry.IsExpired}");   // false
Console.WriteLine();

// --- Test mode: ManualTimeProvider ------------------------------------------

Console.WriteLine("--- Test mode (ManualTimeProvider) ---");

var startTime = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
var fakeProvider = new ManualTimeProvider(startTime);
var testEntry = new CacheEntry<string>("test-value", fakeProvider, TimeSpan.FromMinutes(5));

Console.WriteLine($"Start time       : {fakeProvider.GetUtcNow():O}");
Console.WriteLine($"Is expired?      : {testEntry.IsExpired}");         // false

// Advance time by 3 minutes — still valid
fakeProvider.Advance(TimeSpan.FromMinutes(3));
Console.WriteLine();
Console.WriteLine($"After +3 min     : {fakeProvider.GetUtcNow():O}");
Console.WriteLine($"Is expired?      : {testEntry.IsExpired}");         // false

// Advance time by another 3 minutes — now expired (6 min > 5 min TTL)
fakeProvider.Advance(TimeSpan.FromMinutes(3));
Console.WriteLine();
Console.WriteLine($"After +6 min     : {fakeProvider.GetUtcNow():O}");
Console.WriteLine($"Is expired?      : {testEntry.IsExpired}");         // true
Console.WriteLine();

// --- TimeProvider.CreateTimer for periodic callbacks ------------------------

Console.WriteLine("--- TimeProvider.CreateTimer (periodic callback) ---");

int tickCount = 0;
using var timer = systemProvider.CreateTimer(
    callback: _ =>
    {
        tickCount++;
        Console.WriteLine($"  Timer tick #{tickCount} at {systemProvider.GetUtcNow():HH:mm:ss.fff}");
    },
    state: null,
    dueTime: TimeSpan.Zero,            // fire immediately
    period: TimeSpan.FromMilliseconds(500));

// Let it tick a few times
await Task.Delay(TimeSpan.FromSeconds(2));
timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan); // stop

Console.WriteLine($"Timer stopped after {tickCount} ticks.");
Console.WriteLine();
Console.WriteLine("Done.");

// =============================================================================
// Types — must come after top-level statements
// =============================================================================

class CacheEntry<T>(T value, TimeProvider timeProvider, TimeSpan ttl)
{
    private readonly DateTimeOffset _created = timeProvider.GetUtcNow();
    public T Value => value;
    public bool IsExpired => timeProvider.GetUtcNow() - _created > ttl;
}

class ManualTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow;

    public ManualTimeProvider(DateTimeOffset startTime) => _utcNow = startTime;

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void Advance(TimeSpan duration) => _utcNow += duration;

    public void SetUtcNow(DateTimeOffset value) => _utcNow = value;
}
