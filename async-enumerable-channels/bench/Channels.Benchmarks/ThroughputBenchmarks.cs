using System.Collections.Concurrent;
using System.Threading.Channels;
using BenchmarkDotNet.Attributes;

namespace Channels.Benchmarks;

public class ThroughputBenchmarks
{
    [Params(1_000, 100_000)]
    public int MessageCount { get; set; }

    [Benchmark]
    public async Task UnboundedChannel()
    {
        var channel = Channel.CreateUnbounded<int>();

        // Write all
        for (int i = 0; i < MessageCount; i++)
        {
            channel.Writer.TryWrite(i);
        }
        channel.Writer.Complete();

        // Read all
        await foreach (var _ in channel.Reader.ReadAllAsync())
        {
        }
    }

    [Benchmark]
    public async Task BoundedChannel()
    {
        var channel = Channel.CreateBounded<int>(128);

        var writer = Task.Run(async () =>
        {
            for (int i = 0; i < MessageCount; i++)
            {
                await channel.Writer.WriteAsync(i);
            }
            channel.Writer.Complete();
        });

        await foreach (var _ in channel.Reader.ReadAllAsync())
        {
        }

        await writer;
    }

    [Benchmark]
    public void BlockingCollection()
    {
        var bc = new BlockingCollection<int>(128);

        var producer = Task.Run(() =>
        {
            for (int i = 0; i < MessageCount; i++)
            {
                bc.Add(i);
            }
            bc.CompleteAdding();
        });

        foreach (var _ in bc.GetConsumingEnumerable())
        {
        }

        producer.Wait();
    }
}
