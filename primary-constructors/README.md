# Primary Constructors

Primary constructors (C# 12+) allow you to declare constructor parameters directly on the class or struct declaration. This project investigates capture semantics, the mutable parameter gotcha, and provides IL comparison points against traditional constructors.

## Key Concepts

- **Capture semantics:** Primary constructor parameters are captured by the compiler into hidden fields when used in members.
- **Mutable capture gotcha:** If a parameter is both captured in a method and exposed via a property, the compiler generates two independent backing fields — mutations to one do not affect the other.
- **IL comparison:** Traditional constructors with explicit readonly fields vs. compiler-generated fields from primary constructors.

## Run

```bash
cd primary-constructors/console-app
dotnet run
```

## IL Analysis

```bash
cd primary-constructors/console-app
dotnet build -c Release
# Use ILSpy, ildasm, or sharplab.io to inspect the output assembly
```
