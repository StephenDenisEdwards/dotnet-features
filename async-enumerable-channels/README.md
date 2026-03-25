# async-enumerable-channels

Explores `IAsyncEnumerable<T>` and `System.Threading.Channels` for producer/consumer streaming patterns. Compares Channel<T> vs BlockingCollection<T> vs IAsyncEnumerable for throughput.

## Run

```bash
# Console app
dotnet run --project console-app/console-app.csproj

# Benchmarks
dotnet run --project bench/Channels.Benchmarks/Channels.Benchmarks.csproj -c Release
```
