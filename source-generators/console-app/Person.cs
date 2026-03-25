using SourceGenerators;

namespace ConsoleApp;

[GenerateToString]
public partial class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
