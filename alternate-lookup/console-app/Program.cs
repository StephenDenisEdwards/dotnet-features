// Alternate Lookup — Dictionary lookups by ReadOnlySpan<char> without allocating a string

// 1. Create a Dictionary<string, int> with sample entries
var dictionary = new Dictionary<string, int>
{
    ["apple"] = 1,
    ["banana"] = 2,
    ["cherry"] = 3,
    ["date"] = 4,
    ["elderberry"] = 5
};

// 2. Get an AlternateLookup<ReadOnlySpan<char>> from it
var lookup = dictionary.GetAlternateLookup<ReadOnlySpan<char>>();

// 3. Look up values using ReadOnlySpan<char> — slicing a larger string without allocating
string source = "I like apple and cherry pie";

// Slice out "apple" (indices 7..12) and "cherry" (indices 17..23) — no string allocation
ReadOnlySpan<char> appleSpan = source.AsSpan(7, 5);
ReadOnlySpan<char> cherrySpan = source.AsSpan(17, 6);

Console.WriteLine($"Source string: \"{source}\"");
Console.WriteLine();

if (lookup.TryGetValue(appleSpan, out int appleValue))
    Console.WriteLine($"Looked up span \"{appleSpan}\" -> {appleValue}");

if (lookup.TryGetValue(cherrySpan, out int cherryValue))
    Console.WriteLine($"Looked up span \"{cherrySpan}\" -> {cherryValue}");

// Also try a span that doesn't exist
ReadOnlySpan<char> missing = "mango".AsSpan();
if (!lookup.TryGetValue(missing, out _))
    Console.WriteLine($"Looked up span \"{missing}\" -> not found");

// 4. Print results summary
Console.WriteLine();
Console.WriteLine("All lookups performed via ReadOnlySpan<char> — zero string allocations for the keys.");
