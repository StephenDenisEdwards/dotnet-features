# SIMD Vectorization

`Vector<T>`, `Vector128<T>`, and `Vector256<T>` enable SIMD (Single Instruction, Multiple Data) operations in .NET. `System.Numerics.Vector<T>` provides a portable abstraction whose width adapts to the hardware, while `System.Runtime.Intrinsics` exposes fixed-width vectors (`Vector128<T>`, `Vector256<T>`) for explicit control. These types let you process multiple elements per instruction, dramatically improving throughput for data-parallel workloads such as array aggregation, image processing, and numerical computation.

## Run

```bash
# Console app
dotnet run --project console-app/console-app.csproj

# Benchmarks
dotnet run --project bench/Simd.Benchmarks/Simd.Benchmarks.csproj -c Release
```
