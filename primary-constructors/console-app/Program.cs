Console.WriteLine("=== Primary Constructors — IL Analysis Demo ===");
Console.WriteLine();

// Traditional class
var pt = new PersonTraditional("Alice", 30);
Console.WriteLine($"  {pt}");

// Primary constructor class
var pp = new PersonPrimary("Bob", 25);
Console.WriteLine($"  {pp}");

Console.WriteLine();

// Traditional struct
var st = new PointTraditional(1.5, 2.5);
Console.WriteLine($"  {st}");

// Primary constructor struct
var sp = new PointPrimary(3.0, 4.0);
Console.WriteLine($"  {sp}");

Console.WriteLine();

// Mutable capture gotcha
Console.WriteLine("--- Mutable Capture Gotcha ---");
var gotcha = new MutableCaptureGotcha("hello");
Console.WriteLine($"  Name property before:  {gotcha.Name}");
var modified = gotcha.GetModifiedName();
Console.WriteLine($"  GetModifiedName():     {modified}");
Console.WriteLine($"  Name property after:   {gotcha.Name}");
Console.WriteLine();
Console.WriteLine("  NOTE: Both the property and the method share the same captured field.");
Console.WriteLine("  Reassigning the parameter inside GetModifiedName() DOES affect the");
Console.WriteLine("  property — this is the 'gotcha'. Compare the IL to see the single");
Console.WriteLine("  compiler-generated field backing both usages.");

// =============================================================================
// Types — must come after top-level statements
// =============================================================================

// Traditional constructor with readonly fields
class PersonTraditional
{
    private readonly string _name;
    private readonly int _age;

    public PersonTraditional(string name, int age)
    {
        _name = name;
        _age = age;
    }

    public string Name => _name;
    public int Age => _age;

    public override string ToString() => $"PersonTraditional {{ Name = {Name}, Age = {Age} }}";
}

// Primary constructor on a class
class PersonPrimary(string name, int age)
{
    public string Name => name;
    public int Age => age;

    public override string ToString() => $"PersonPrimary {{ Name = {Name}, Age = {Age} }}";
}

// Traditional constructor on a struct
struct PointTraditional
{
    private readonly double _x;
    private readonly double _y;

    public PointTraditional(double x, double y)
    {
        _x = x;
        _y = y;
    }

    public double X => _x;
    public double Y => _y;

    public override string ToString() => $"PointTraditional {{ X = {X}, Y = {Y} }}";
}

// Primary constructor on a struct
struct PointPrimary(double x, double y)
{
    public double X => x;
    public double Y => y;

    public override string ToString() => $"PointPrimary {{ X = {X}, Y = {Y} }}";
}

// Mutable capture gotcha — parameter is shared between property and method
class MutableCaptureGotcha(string name)
{
    public string Name => name;

    public string GetModifiedName()
    {
        name = name.ToUpperInvariant();
        return name;
    }
}
