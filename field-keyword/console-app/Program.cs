// Investigating the C# 14 'field' keyword for auto-property backing field access.
// The 'field' keyword lets you access the compiler-generated backing field directly
// inside a property accessor, eliminating the need for explicit backing fields.

// --- Traditional approach: explicit backing fields with validation ---

var traditional = new PersonTraditional("Alice", 30);
Console.WriteLine($"Traditional: {traditional.Name}, Age {traditional.Age}");

try
{
    traditional.Name = null!;
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Traditional caught: {ex.ParamName} cannot be null");
}

try
{
    traditional.Age = -1;
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Traditional caught: {ex.ParamName} out of range");
}

// --- Modern approach: using the 'field' keyword ---

var modern = new PersonModern("Bob", 25);
Console.WriteLine($"Modern: {modern.Name}, Age {modern.Age}");

try
{
    modern.Name = null!;
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Modern caught: {ex.ParamName} cannot be null");
}

try
{
    modern.Age = -1;
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Modern caught: {ex.ParamName} out of range");
}

Console.WriteLine("Done.");

// Traditional: explicit backing fields + validation
public class PersonTraditional
{
    private string _name;
    private int _age;

    public PersonTraditional(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(Name));
    }

    public int Age
    {
        get => _age;
        set => _age = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Age), "Age must be non-negative");
    }
}

// Modern: the 'field' keyword replaces explicit backing fields.
// 'field' refers to the compiler-generated backing field for the property.
// NOTE: Requires C# 14 / LangVersion preview. If your SDK does not yet support
// the 'field' keyword, this will fail to compile.
public class PersonModern
{
    public PersonModern(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string Name
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(Name));
    }

    public int Age
    {
        get;
        set => field = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Age), "Age must be non-negative");
    }
}
