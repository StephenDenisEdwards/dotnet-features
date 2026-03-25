using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

const int ItemCount = 1_000;

// --- 1. IAsyncEnumerable ---
Console.WriteLine("=== IAsyncEnumerable ===");
var sw = Stopwatch.StartNew();
int count = 0;
await foreach (var item in ProduceAsync(ItemCount))
{
    count++;
}
sw.Stop();
Console.WriteLine($"  Consumed {count} items in {sw.Elapsed.TotalMilliseconds:F2} ms");

// --- 2. Channel<T> ---
Console.WriteLine();
Console.WriteLine("=== Channel<T> (Bounded, capacity 100) ===");
sw.Restart();
var channel = Channel.CreateBounded<int>(100);
var writerTask = Task.Run(async () =>
{
    for (int i = 0; i < ItemCount; i++)
    {
        await channel.Writer.WriteAsync(i);
    }
    channel.Writer.Complete();
});

count = 0;
await foreach (var item in channel.Reader.ReadAllAsync())
{
    count++;
}
await writerTask;
sw.Stop();
Console.WriteLine($"  Consumed {count} items in {sw.Elapsed.TotalMilliseconds:F2} ms");

// --- 3. BlockingCollection<T> ---
Console.WriteLine();
Console.WriteLine("=== BlockingCollection<T> ===");
sw.Restart();
var bc = new BlockingCollection<int>(100);
var producerThread = Task.Run(() =>
{
    for (int i = 0; i < ItemCount; i++)
    {
        bc.Add(i);
    }
    bc.CompleteAdding();
});

count = 0;
foreach (var item in bc.GetConsumingEnumerable())
{
    count++;
}
await producerThread;
sw.Stop();
Console.WriteLine($"  Consumed {count} items in {sw.Elapsed.TotalMilliseconds:F2} ms");

Console.WriteLine();
Console.WriteLine("Done.");

static async IAsyncEnumerable<int> ProduceAsync(int total, [EnumeratorCancellation] CancellationToken ct = default)
{
    for (int i = 0; i < total; i++)
    {
        yield return i;
    }
    await Task.CompletedTask; // keep the method truly async-shaped
}
