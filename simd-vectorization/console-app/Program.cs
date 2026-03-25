using System.Numerics;
using System.Runtime.Intrinsics;

// =============================================================================
// Hardware capabilities
// =============================================================================

Console.WriteLine("=== SIMD Hardware Info ===");
Console.WriteLine();
Console.WriteLine($"  Vector<float>.Count     = {Vector<float>.Count} floats ({Vector<float>.Count * 4 * 8}-bit registers)");
Console.WriteLine($"  Vector128 accelerated   = {Vector128.IsHardwareAccelerated}");
Console.WriteLine($"  Vector256 accelerated   = {Vector256.IsHardwareAccelerated}");
Console.WriteLine($"  Vector512 accelerated   = {Vector512.IsHardwareAccelerated}");
Console.WriteLine();

// =============================================================================
// Test data
// =============================================================================

const int Length = 1024;
float[] data = new float[Length];
var rng = new Random(42);
for (int i = 0; i < data.Length; i++)
    data[i] = rng.NextSingle() * 100f;

// =============================================================================
// 1. Sum — the simplest SIMD operation
//    Each iteration adds N floats at once instead of 1.
// =============================================================================

Console.WriteLine("=== Sum Benchmark (quick) ===");
Console.WriteLine();

float scalarSum = ScalarSum(data);
float vectorSum = VectorSum(data);
Console.WriteLine($"  Scalar sum:      {scalarSum:F4}");
Console.WriteLine($"  Vector<T> sum:   {vectorSum:F4}");

if (Vector256.IsHardwareAccelerated)
{
    float v256Sum = Vector256Sum(data);
    Console.WriteLine($"  Vector256 sum:   {v256Sum:F4}");
}

Console.WriteLine();
Console.WriteLine("  Note: small floating-point differences are expected — SIMD changes");
Console.WriteLine("  the order of additions, which affects FP rounding.");
Console.WriteLine();

// =============================================================================
// 2. Element-wise multiply — transform each element in-place
//    Simulates operations like audio gain, image brightness, physics scaling.
// =============================================================================

Console.WriteLine("=== Element-wise Multiply ===");
Console.WriteLine();

float[] scalarResult = new float[Length];
float[] vectorResult = new float[Length];
const float multiplier = 2.5f;

ScalarMultiply(data, scalarResult, multiplier);
VectorMultiply(data, vectorResult, multiplier);

bool allMatch = true;
for (int i = 0; i < Length; i++)
{
    if (scalarResult[i] != vectorResult[i])
    {
        allMatch = false;
        break;
    }
}

Console.WriteLine($"  Multiplied {Length} floats by {multiplier}");
Console.WriteLine($"  Scalar[0..4]:  [{scalarResult[0]:F2}, {scalarResult[1]:F2}, {scalarResult[2]:F2}, {scalarResult[3]:F2}]");
Console.WriteLine($"  Vector[0..4]:  [{vectorResult[0]:F2}, {vectorResult[1]:F2}, {vectorResult[2]:F2}, {vectorResult[3]:F2}]");
Console.WriteLine($"  Results match: {allMatch}");
Console.WriteLine();

// =============================================================================
// 3. Find max — horizontal reduction across SIMD lanes
//    SIMD compares N elements at once, but the final max must be extracted
//    from the vector lanes — this is the "horizontal reduction" step.
// =============================================================================

Console.WriteLine("=== Find Max (horizontal reduction) ===");
Console.WriteLine();

float scalarMax = ScalarMax(data);
float vectorMax = VectorMax(data);

Console.WriteLine($"  Scalar max:    {scalarMax:F4}");
Console.WriteLine($"  Vector<T> max: {vectorMax:F4}");
Console.WriteLine($"  Match: {scalarMax == vectorMax}");
Console.WriteLine();

// =============================================================================
// 4. Contains — SIMD search with early exit
//    Checks N elements per iteration for equality. Shows the trade-off:
//    SIMD has higher throughput but the comparison + mask check adds per-
//    iteration cost vs a simple scalar branch.
// =============================================================================

Console.WriteLine("=== Contains (SIMD search) ===");
Console.WriteLine();

float target = data[Length - 10]; // near the end
bool scalarFound = ScalarContains(data, target);
bool vectorFound = VectorContains(data, target);

Console.WriteLine($"  Looking for {target:F4} in {Length} elements");
Console.WriteLine($"  Scalar found: {scalarFound}");
Console.WriteLine($"  Vector found: {vectorFound}");
Console.WriteLine();

float missing = -999.99f;
Console.WriteLine($"  Looking for {missing:F2} (not in array)");
Console.WriteLine($"  Scalar found: {ScalarContains(data, missing)}");
Console.WriteLine($"  Vector found: {VectorContains(data, missing)}");
Console.WriteLine();

Console.WriteLine("Run the benchmarks for detailed performance numbers:");
Console.WriteLine("  dotnet run -c Release --project simd-vectorization/bench/Simd.Benchmarks -- --filter \"*\"");

// =============================================================================
// Implementations
// =============================================================================

static float ScalarSum(float[] array)
{
    float sum = 0f;
    for (int i = 0; i < array.Length; i++)
        sum += array[i];
    return sum;
}

static float VectorSum(float[] array)
{
    int vectorSize = Vector<float>.Count;
    int i = 0;
    var vSum = Vector<float>.Zero;

    for (; i <= array.Length - vectorSize; i += vectorSize)
        vSum += new Vector<float>(array, i);

    float sum = Vector.Dot(vSum, Vector<float>.One);

    for (; i < array.Length; i++)
        sum += array[i];

    return sum;
}

static float Vector256Sum(float[] array)
{
    int vectorSize = Vector256<float>.Count;
    int i = 0;
    var vSum = Vector256<float>.Zero;

    ReadOnlySpan<float> span = array;
    for (; i <= span.Length - vectorSize; i += vectorSize)
        vSum = Vector256.Add(vSum, Vector256.Create(span.Slice(i, vectorSize)));

    float sum = Vector256.Sum(vSum);

    for (; i < array.Length; i++)
        sum += array[i];

    return sum;
}

static void ScalarMultiply(float[] source, float[] destination, float multiplier)
{
    for (int i = 0; i < source.Length; i++)
        destination[i] = source[i] * multiplier;
}

static void VectorMultiply(float[] source, float[] destination, float multiplier)
{
    int vectorSize = Vector<float>.Count;
    var vMul = new Vector<float>(multiplier);
    int i = 0;

    for (; i <= source.Length - vectorSize; i += vectorSize)
    {
        var v = new Vector<float>(source, i);
        (v * vMul).CopyTo(destination, i);
    }

    for (; i < source.Length; i++)
        destination[i] = source[i] * multiplier;
}

static float ScalarMax(float[] array)
{
    float max = float.MinValue;
    for (int i = 0; i < array.Length; i++)
    {
        if (array[i] > max)
            max = array[i];
    }
    return max;
}

static float VectorMax(float[] array)
{
    int vectorSize = Vector<float>.Count;
    var vMax = new Vector<float>(float.MinValue);
    int i = 0;

    for (; i <= array.Length - vectorSize; i += vectorSize)
        vMax = Vector.Max(vMax, new Vector<float>(array, i));

    float max = float.MinValue;
    for (int lane = 0; lane < vectorSize; lane++)
    {
        if (vMax[lane] > max)
            max = vMax[lane];
    }

    for (; i < array.Length; i++)
    {
        if (array[i] > max)
            max = array[i];
    }

    return max;
}

static bool ScalarContains(float[] array, float target)
{
    for (int i = 0; i < array.Length; i++)
    {
        if (array[i] == target)
            return true;
    }
    return false;
}

static bool VectorContains(float[] array, float target)
{
    int vectorSize = Vector<float>.Count;
    var vTarget = new Vector<float>(target);
    int i = 0;

    for (; i <= array.Length - vectorSize; i += vectorSize)
    {
        var v = new Vector<float>(array, i);
        if (Vector.EqualsAny(v, vTarget))
            return true;
    }

    for (; i < array.Length; i++)
    {
        if (array[i] == target)
            return true;
    }

    return false;
}
