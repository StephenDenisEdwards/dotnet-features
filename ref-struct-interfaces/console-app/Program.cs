using System.Numerics;

// Use ResourceHandle in a using statement
using (var handle = new ResourceHandle())
{
    Console.WriteLine($"Handle disposed: {handle.IsDisposed}");
}

// Use Sum with Span (ref struct in generic context)
ReadOnlySpan<int> numbers = [1, 2, 3, 4, 5];
Console.WriteLine($"Sum: {Sum(numbers)}");

// A generic method that accepts ReadOnlySpan<T> with INumber<T>
static T Sum<T>(ReadOnlySpan<T> values) where T : INumber<T>
{
    T sum = T.Zero;
    foreach (var v in values)
        sum += v;
    return sum;
}

// A ref struct that implements IDisposable
ref struct ResourceHandle : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
        Console.WriteLine("ResourceHandle disposed");
    }
}
