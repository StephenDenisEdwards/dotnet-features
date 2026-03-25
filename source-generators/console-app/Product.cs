using SourceGenerators;

namespace ConsoleApp;

[GenerateToString]
public partial class Product
{
    public string Sku { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
