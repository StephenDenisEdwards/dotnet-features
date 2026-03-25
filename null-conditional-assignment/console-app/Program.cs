// =============================================================================
// C# 14 / .NET 11 - Null-Conditional Assignment (?.= operator)
// =============================================================================
// Previously, assigning to a member of a potentially null reference required an
// explicit null check. C# 14 extends the ?. operator to support the left-hand
// side of an assignment, short-circuiting when the receiver is null.
// =============================================================================

Console.WriteLine("=== Null-Conditional Assignment ===\n");

// ---------------------------------------------------------------------------
// Supporting types
// ---------------------------------------------------------------------------

var personA = new Person { Name = "Initial", Address = new Address { City = "Initial" } };
var personB = new Person { Name = "Initial", Address = new Address { City = "Initial" } };
Person? nullPerson = null;

// ---------------------------------------------------------------------------
// 1. Traditional pattern: explicit null check before assignment
// ---------------------------------------------------------------------------

Console.WriteLine("--- Traditional null-check pattern ---");

if (personA != null)
    personA.Name = "Alice";

if (personA?.Address != null)
    personA.Address.City = "London";

Console.WriteLine($"personA.Name    = {personA.Name}");
Console.WriteLine($"personA.Address.City = {personA.Address?.City}");

// ---------------------------------------------------------------------------
// 2. New pattern: null-conditional assignment (C# 14)
// ---------------------------------------------------------------------------

Console.WriteLine("\n--- Null-conditional assignment pattern ---");

personB?.Name = "Alice";
personB?.Address?.City = "London";

Console.WriteLine($"personB.Name    = {personB.Name}");
Console.WriteLine($"personB.Address.City = {personB.Address?.City}");

// ---------------------------------------------------------------------------
// 3. Verify equivalence
// ---------------------------------------------------------------------------

Console.WriteLine("\n--- Equivalence check ---");
Console.WriteLine($"Names match:  {personA.Name == personB.Name}");
Console.WriteLine($"Cities match: {personA.Address?.City == personB.Address?.City}");

// ---------------------------------------------------------------------------
// 4. Null receiver: assignment is safely skipped
// ---------------------------------------------------------------------------

Console.WriteLine("\n--- Null receiver (no-op) ---");

nullPerson?.Name = "Should not throw";
nullPerson?.Address?.City = "Should not throw";

Console.WriteLine($"nullPerson is still null: {nullPerson is null}");

Console.WriteLine("\nDone.");

// =============================================================================
// Type definitions
// =============================================================================

public class Address
{
    public string? City { get; set; }
}

public class Person
{
    public string? Name { get; set; }
    public Address? Address { get; set; }
}
