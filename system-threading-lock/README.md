# system-threading-lock

Benchmarks the .NET 9+ `System.Threading.Lock` type against `Monitor` (`lock` statement), `SemaphoreSlim`, and `SpinLock` for uncontended locking scenarios.

## Run Benchmarks

From the repository root:

```powershell
dotnet run -c Release --project .\system-threading-lock\bench\Lock.Benchmarks\Lock.Benchmarks.csproj -- --filter *
```
