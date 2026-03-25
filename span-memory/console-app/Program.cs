// -------------------------------------------------------
// Span<T> and Memory<T> — feature investigation
// -------------------------------------------------------

Console.WriteLine("=== 1. Parsing an int: Span vs string.Substring ===\n");

string input = "Temperature:42";

// Substring approach — allocates a new string
string numberStr = input.Substring("Temperature:".Length);
int parsed1 = int.Parse(numberStr);
Console.WriteLine($"  Substring  → {parsed1}  (allocated \"{numberStr}\")");

// Span approach — zero allocation, slices into the existing string
ReadOnlySpan<char> span = input.AsSpan();
int colonIndex = span.IndexOf(':');
ReadOnlySpan<char> numberSpan = span[(colonIndex + 1)..];
int parsed2 = int.Parse(numberSpan);
Console.WriteLine($"  Span.Slice → {parsed2}  (no allocation)");

Console.WriteLine();
Console.WriteLine("=== 2. stackalloc with Span ===\n");

Span<byte> buffer = stackalloc byte[16];
for (int i = 0; i < buffer.Length; i++)
    buffer[i] = (byte)(i * 3);

Console.Write("  Stack buffer: ");
for (int i = 0; i < buffer.Length; i++)
    Console.Write($"{buffer[i],4}");
Console.WriteLine();
Console.WriteLine("  (allocated on stack — no GC pressure)");

Console.WriteLine();
Console.WriteLine("=== 3. Slicing an array: Span vs Array.Copy ===\n");

int[] source = Enumerable.Range(0, 20).ToArray();

// Array.Copy — allocates a new array
int[] copied = new int[5];
Array.Copy(source, 5, copied, 0, 5);
Console.WriteLine($"  Array.Copy → [{string.Join(", ", copied)}]  (new array allocated)");

// Span.Slice — zero-copy view over the original array
Span<int> sliced = source.AsSpan(5, 5);
Console.Write("  Span.Slice → [");
for (int i = 0; i < sliced.Length; i++)
{
    if (i > 0) Console.Write(", ");
    Console.Write(sliced[i]);
}
Console.WriteLine("]  (no allocation, view over original)");

Console.WriteLine();
Console.WriteLine("=== 4. Memory<T> for async-compatible slicing ===\n");

int[] data = Enumerable.Range(1, 10).ToArray();
Memory<int> memory = data.AsMemory(2, 5);

Console.WriteLine($"  Original array: [{string.Join(", ", data)}]");
Console.WriteLine($"  Memory slice:   [{string.Join(", ", memory.ToArray())}]");

int sum = await SumAsync(memory);
Console.WriteLine($"  Async sum of slice: {sum}");
Console.WriteLine("  (Memory<T> can cross await boundaries, unlike Span<T>)");

static async Task<int> SumAsync(Memory<int> memory)
{
    await Task.Yield(); // simulate async work
    int total = 0;
    // Convert to Span inside synchronous scope
    Span<int> span = memory.Span;
    for (int i = 0; i < span.Length; i++)
        total += span[i];
    return total;
}
