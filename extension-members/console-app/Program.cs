// Extension Members in C# 14 - comparing traditional and new syntax
//
// Both approaches produce the same usage at the call site for methods,
// but the new extension member syntax in C# 14 also allows extension
// properties and a more natural declaration style.

using ExtensionMembers.Modern;
// To compare with traditional syntax, swap to:
// using ExtensionMembers.Traditional;

var text = "Hello, this is a long string that we want to truncate";

// --- Extension methods work the same way in both Traditional and Modern ---
Console.WriteLine("=== Extension Methods (work in both Traditional & Modern) ===");
Console.WriteLine($"Original : {text}");
Console.WriteLine($"Truncated: {text.Truncate(20)}");
Console.WriteLine();

// --- Extension properties are NEW in C# 14 (Modern only) ---
// With the Traditional approach, IsNullOrWhitespace is a method: "".IsNullOrWhitespace()
// With the Modern approach, it is a property: "".IsNullOrWhitespace (no parentheses!)
Console.WriteLine("=== Extension Properties (C# 14 - Modern only) ===");
Console.WriteLine($"IsNullOrWhitespace on \"\"   : {"".IsNullOrWhitespace}");
Console.WriteLine($"IsNullOrWhitespace on \"hi\" : {"hi".IsNullOrWhitespace}");
Console.WriteLine();

string? nullStr = null;
Console.WriteLine($"IsNullOrWhitespace on null : {nullStr.IsNullOrWhitespace}");
Console.WriteLine($"IsNullOrWhitespace on \"  \" : {"  ".IsNullOrWhitespace}");
