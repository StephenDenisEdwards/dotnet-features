using System.Numerics;

// ---------------------------------------------------------------------------
// 1. Generic Sum<T> with INumber<T> and ReadOnlySpan<T>
//    Note: INumber<T> itself doesn't need 'allows ref struct' since numeric
//    types are never ref structs. The interesting part is ReadOnlySpan<T> as
//    a parameter with a generic element type.
// ---------------------------------------------------------------------------

static T Sum<T>(ReadOnlySpan<T> values) where T : INumber<T>
{
    T result = T.Zero;
    foreach (T value in values)
        result += value;
    return result;
}

ReadOnlySpan<int> numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
Console.WriteLine($"Sum of 1..10: {Sum(numbers)}");

ReadOnlySpan<double> doubles = [1.1, 2.2, 3.3];
Console.WriteLine($"Sum of doubles: {Sum(doubles)}");

// ---------------------------------------------------------------------------
// 2. Generic Print<T> with allows ref struct
//    The 'allows ref struct' constraint lets this method accept Span<T>,
//    ReadOnlySpan<char>, and regular types alike.
// ---------------------------------------------------------------------------

static void Consume<T>(T value) where T : allows ref struct
{
    // With 'allows ref struct', T can't be boxed to object — so we can't
    // pass it to Console.WriteLine(object). This is the key trade-off:
    // you gain the ability to accept ref structs, but lose boxing-dependent APIs.
    Console.WriteLine($"[Consume<{typeof(T).Name}>] received a value (size on stack: {System.Runtime.CompilerServices.Unsafe.SizeOf<T>()} bytes)");
}

// Call with a regular string (not a ref struct — pointer-sized)
Consume("plain string value");

// Call with an int (4 bytes)
Consume(42);

// Call with a Guid (16 bytes)
Consume(Guid.NewGuid());

// ---------------------------------------------------------------------------
// 3. Interface with allows ref struct type parameter
// ---------------------------------------------------------------------------

Console.WriteLine();
Console.WriteLine("--- IBufferConsumer<ReadOnlySpan<byte>> ---");

SpanBufferConsumer consumer = new();
ReadOnlySpan<byte> payload = [0xCA, 0xFE, 0xBA, 0xBE, 0x00, 0xFF];
consumer.Consume(payload);

// ---------------------------------------------------------------------------
// 4. Generic method constrained to allows ref struct + IDisposable
//    Shows that ref structs implementing IDisposable can be used generically
// ---------------------------------------------------------------------------

Console.WriteLine();
Console.WriteLine("--- allows ref struct + IDisposable ---");

static void UseAndDispose<T>(T resource) where T : IDisposable, allows ref struct
{
    Console.WriteLine($"  Using resource of type: {typeof(T).Name}");
    resource.Dispose();
    Console.WriteLine("  Disposed.");
}

// Use with a regular IDisposable
using var ms = new MemoryStream();
// Note: Can't call UseAndDispose with a ref struct through the generic path
// in current preview — demonstrate with a class-based IDisposable
UseAndDispose(new MemoryStream());

// ---------------------------------------------------------------------------
// Supporting types
// ---------------------------------------------------------------------------

interface IBufferConsumer<T> where T : allows ref struct
{
    void Consume(T buffer);
}

class SpanBufferConsumer : IBufferConsumer<ReadOnlySpan<byte>>
{
    public void Consume(ReadOnlySpan<byte> buffer)
    {
        Console.Write($"Consumed {buffer.Length} bytes: ");
        foreach (byte b in buffer)
            Console.Write($"0x{b:X2} ");
        Console.WriteLine();
    }
}
