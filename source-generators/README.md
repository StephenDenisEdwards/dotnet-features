# Source Generators

## What Are Source Generators?

Source generators are a C# compiler feature (introduced in .NET 5 / Roslyn 3.8) that lets you
inspect user code at compile time and emit additional C# source files that are added to the
compilation. Unlike reflection, all the work happens at build time, so there is zero runtime
overhead.

Key characteristics:

- **Additive only** -- generators can add new source files but cannot modify existing code.
- **Run at compile time** -- the generated code is ordinary C# that participates in type-checking
  and IntelliSense.
- **Incremental** -- the `IIncrementalGenerator` API (recommended since .NET 6) caches intermediate
  results so that regeneration only happens when the relevant inputs change.

## What This Project Demonstrates

The `generator` project contains a **`ToStringGenerator`** that:

1. Emits a `[GenerateToString]` marker attribute (so consumers don't need a separate dependency).
2. Finds every `partial class` or `partial struct` annotated with `[GenerateToString]`.
3. Generates a `ToString()` override that prints all public properties.

For example, given:

```csharp
[GenerateToString]
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
```

The generator produces:

```csharp
partial class Person
{
    public override string ToString() => $"Person { Name = {Name}, Age = {Age} }";
}
```

The `console-app` project references the generator as an analyzer and uses it on two model
classes (`Person` and `Product`).

## How to Run

```bash
dotnet run --project source-generators/console-app
```

Expected output:

```
Person { Name = Alice, Age = 30 }
Product { Sku = WIDGET-42, Description = Premium Widget, Price = 19.99 }
```

## Inspecting Generated Code

After building, you can find the generated files under:

```
source-generators/console-app/obj/Debug/net10.0/generated/SourceGenerators.Generator/SourceGenerators.Generator.ToStringGenerator/
```

You will see:

- `GenerateToStringAttribute.g.cs` -- the marker attribute.
- `ConsoleApp.Person.g.cs` -- the generated `ToString()` for `Person`.
- `ConsoleApp.Product.g.cs` -- the generated `ToString()` for `Product`.

In Visual Studio or Rider you can also expand **Dependencies > Analyzers >
SourceGenerators.Generator** in Solution Explorer to browse the generated files directly.
