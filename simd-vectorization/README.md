# SIMD Vectorization

SIMD (Single Instruction, Multiple Data) lets the CPU apply one operation to multiple data elements in a single clock cycle. Instead of adding two floats at a time in a loop, SIMD adds 4, 8, or even 16 floats simultaneously — depending on the hardware register width.

.NET exposes SIMD through three layers, each trading portability for control:

## The three SIMD layers in .NET

### 1. `Vector<T>` (System.Numerics) — portable, width-adaptive

`Vector<T>` is the simplest entry point. Its width adapts automatically to whatever the hardware supports — 128-bit on older CPUs, 256-bit on AVX2 machines, 512-bit on AVX-512. You write one code path and the JIT picks the widest available registers.

```csharp
// Vector<float>.Count tells you how many floats fit in one register.
// On AVX2 hardware this is 8 (256 bits / 32 bits per float).
var vSum = Vector<float>.Zero;
for (int i = 0; i <= data.Length - Vector<float>.Count; i += Vector<float>.Count)
    vSum += new Vector<float>(data, i);
float total = Vector.Dot(vSum, Vector<float>.One);
```

**Pros:** Zero platform-specific code, works everywhere .NET runs.
**Cons:** You can't control the exact register width, and the API surface is smaller than the intrinsics layer.

### 2. `Vector128<T>` / `Vector256<T>` / `Vector512<T>` (System.Runtime.Intrinsics) — fixed-width

These types give you explicit control over the register width. `Vector256<float>` always uses 256-bit registers (8 floats), regardless of what the hardware *could* support. The JIT inserts the appropriate instructions (SSE, AVX2, NEON, etc.) and you get software fallbacks when the width isn't natively supported.

```csharp
var vSum = Vector256<float>.Zero;
ReadOnlySpan<float> span = data;
for (int i = 0; i <= span.Length - Vector256<float>.Count; i += Vector256<float>.Count)
    vSum = Vector256.Add(vSum, Vector256.Create(span.Slice(i, 8)));
float total = Vector256.Sum(vSum);
```

**Pros:** Deterministic width, access to operations not on `Vector<T>` (shuffles, bitwise blends, etc.).
**Cons:** You may need multiple code paths (256 + 128 fallback) for best performance across hardware.

### 3. Hardware intrinsics (`Sse2`, `Avx2`, `AdvSimd`, etc.) — raw ISA instructions

The lowest level maps 1:1 to CPU instructions. You check `Avx2.IsSupported` and call methods like `Avx2.Add`, `Avx2.CompareGreaterThan`, etc. This is how the .NET runtime itself implements `Span.IndexOf`, `SearchValues`, and other hot paths.

This project focuses on layers 1 and 2, which cover the vast majority of use cases.

## How the JIT auto-vectorizes (and when it doesn't)

Starting with .NET 8, the JIT compiler can auto-vectorize some simple loops. For example, a plain `for` loop summing an `int[]` may be emitted with SIMD instructions automatically. However, auto-vectorization is limited to straightforward patterns — it won't help with:

- Loops with complex control flow (branches, early exits)
- Data-dependent operations (conditional accumulation)
- Non-contiguous memory access patterns
- Reduction operations with floating-point precision concerns

When the JIT can't auto-vectorize, manual `Vector<T>` or `Vector256<T>` code gives you the same throughput benefit with full control.

## What the benchmarks measure

The benchmark project includes several scenarios that highlight where SIMD shines:

### Sum (aggregation)
The simplest SIMD use case — sum all elements of a `float[]`. Compares scalar loop, `Vector<T>`, `Vector256<T>`, and LINQ's `.Sum()`. This is a pure ALU-bound workload where SIMD should scale nearly linearly with register width.

### Element-wise multiply (transform)
Multiply each element of a `float[]` by a constant and write the result to an output array. This is a throughput benchmark — SIMD processes N elements per instruction vs 1 for scalar. Common in audio processing, image brightness adjustment, and physics simulations.

### Find max (horizontal reduction)
Find the maximum value in a `float[]`. SIMD compares N elements at once using `Vector.Max` / `Vector256.Max`, but needs a final horizontal reduction across lanes. Shows that SIMD isn't free — the reduction step adds overhead that matters for small arrays.

### Contains value (search / early exit)
Search for a specific float value in an array. The scalar version can exit early on the first match; the SIMD version checks N elements per iteration but the comparison and mask extraction add per-iteration cost. Demonstrates the trade-off between throughput and early-exit optimisation.

## Key takeaways

- **Array size matters.** SIMD has setup cost (loading vectors, handling remainders). For tiny arrays (< 32 elements), scalar code wins. The crossover point varies by operation but is typically 64–256 elements.
- **Memory bandwidth is the ceiling.** Once the data doesn't fit in L1/L2 cache, SIMD throughput is limited by memory speed, not ALU speed. The large array sizes in the benchmarks show this flattening effect.
- **LINQ `.Sum()` is slow** because it uses `IEnumerable<T>` iteration with interface dispatch on every element. It's convenient, not fast.
- **`Vector<T>` ≈ `Vector256<T>` on AVX2 hardware** because `Vector<T>` adapts to 256-bit width. On ARM/NEON it would be 128-bit.
- **Zero allocations.** All SIMD types are stack-allocated value types. The `[MemoryDiagnoser]` benchmarks confirm 0 bytes allocated for SIMD paths.

## Run

```bash
# Console app — quick demo with correctness check
dotnet run --project simd-vectorization/console-app

# Benchmarks — full performance comparison
dotnet run -c Release --project simd-vectorization/bench/Simd.Benchmarks -- --filter "*"
```
