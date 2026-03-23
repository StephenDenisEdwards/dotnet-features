# Benchmarks

Run these from the `bench` directory:

```powershell
dotnet run -c Release --project .\AsyncCases.Net10.Baseline.Benchmarks\AsyncCases.Net10.Baseline.Benchmarks.csproj -- --filter *
dotnet run -c Release --project .\AsyncCases.Net11.RuntimeAsync.Benchmarks\AsyncCases.Net11.RuntimeAsync.Benchmarks.csproj -- --filter *
```

What they do:

- `AsyncCases.Net10.Baseline.Benchmarks`: baseline benchmark on `net10.0`
- `AsyncCases.Net11.RuntimeAsync.Benchmarks`: runtime-async benchmark on `net11.0`

Notes:

- `-c Release` is required, otherwise BenchmarkDotNet rejects the run as non-optimized.
- `-- --filter *` skips the interactive benchmark picker and runs everything.
- BenchmarkDotNet outputs results under `BenchmarkDotNet.Artifacts`.

# Console Apps

These are small side-by-side demo apps that use the same source and exist to show the compiler/runtime differences, not to produce benchmark numbers.

- `console-net10-app`: targets `net10.0`
- `console-net11-app`: targets `net11.0` with `runtime-async=on`

Run them from the `net11-runtime-async` directory:

```powershell
dotnet run --project .\console-net10-app\console-net10-app.csproj
dotnet run --project .\console-net11-app\console-net11-app.csproj
```

The useful comparison is in the emitted IL. Build the apps and inspect the generated assemblies to see what the compiler emits for each version.
