using System.Numerics;
using System.Runtime.Intrinsics;

const int Length = 1024;
float[] data = new float[Length];
var rng = new Random(42);
for (int i = 0; i < data.Length; i++)
    data[i] = rng.NextSingle() * 100f;

// 1. Scalar sum
float scalarSum = ScalarSum(data);
Console.WriteLine($"Scalar sum:      {scalarSum}");

// 2. Vector<float> sum (System.Numerics — portable SIMD)
float vectorSum = VectorSum(data);
Console.WriteLine($"Vector<T> sum:   {vectorSum}");

// 3. Vector256<float> sum (System.Runtime.Intrinsics — fixed-width SIMD)
if (Vector256.IsHardwareAccelerated)
{
    float vector256Sum = Vector256Sum(data);
    Console.WriteLine($"Vector256 sum:   {vector256Sum}");
}
else
{
    Console.WriteLine("Vector256<float> is NOT hardware-accelerated on this machine.");
}

// 4. Print SIMD width info
Console.WriteLine();
Console.WriteLine($"Vector<float>.Count = {Vector<float>.Count} (floats per SIMD register)");
Console.WriteLine($"Vector256.IsHardwareAccelerated = {Vector256.IsHardwareAccelerated}");

// --- Implementations ---

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

    // Handle remaining elements
    for (; i < array.Length; i++)
        sum += array[i];

    return sum;
}

static float Vector256Sum(float[] array)
{
    int vectorSize = Vector256<float>.Count; // 8 for 256-bit
    int i = 0;
    var vSum = Vector256<float>.Zero;

    ReadOnlySpan<float> span = array;
    for (; i <= span.Length - vectorSize; i += vectorSize)
        vSum = Vector256.Add(vSum, Vector256.Create(span.Slice(i, vectorSize)));

    float sum = Vector256.Sum(vSum);

    // Handle remaining elements
    for (; i < array.Length; i++)
        sum += array[i];

    return sum;
}
