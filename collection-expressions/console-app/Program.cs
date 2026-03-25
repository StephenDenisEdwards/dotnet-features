using System.Collections.Immutable;

// =============================================================================
// Collection Expressions — IL Analysis Demo
// =============================================================================

Console.WriteLine("=== Collection Expressions Demo ===");
Console.WriteLine();

// 1. Array creation
int[] a = [1, 2, 3];
Console.WriteLine($"  int[] a = [1, 2, 3]          -> Length: {a.Length}, Values: [{string.Join(", ", a)}]");

// 2. List creation
List<int> b = [1, 2, 3];
Console.WriteLine($"  List<int> b = [1, 2, 3]      -> Count: {b.Count}, Values: [{string.Join(", ", b)}]");

// 3. Span creation
Span<int> c = [1, 2, 3];
Console.WriteLine($"  Span<int> c = [1, 2, 3]      -> Length: {c.Length}, Values: [{c[0]}, {c[1]}, {c[2]}]");

// 4. ReadOnlySpan creation
ReadOnlySpan<int> d = [1, 2, 3];
Console.WriteLine($"  ReadOnlySpan<int> d = [1,2,3] -> Length: {d.Length}, Values: [{d[0]}, {d[1]}, {d[2]}]");

// 5. Spread operator
int[] combined = [..a, 4, 5, ..new int[] { 6, 7 }];
Console.WriteLine($"  [..a, 4, 5, ..new[] {{6,7}}]  -> Length: {combined.Length}, Values: [{string.Join(", ", combined)}]");

// 6. ImmutableArray
ImmutableArray<int> e = [1, 2, 3];
Console.WriteLine($"  ImmutableArray<int> e = [1,2,3] -> Length: {e.Length}, Values: [{string.Join(", ", e)}]");

Console.WriteLine();
Console.WriteLine("NOTE: Inspect the IL to see how the compiler backs each target type differently.");
Console.WriteLine("  - int[] and List<int> allocate on the heap.");
Console.WriteLine("  - ReadOnlySpan<int> of constants may use static data (RuntimeHelpers.CreateSpan).");
Console.WriteLine("  - Span<int> typically stack-allocates or uses an inline array for small sizes.");
