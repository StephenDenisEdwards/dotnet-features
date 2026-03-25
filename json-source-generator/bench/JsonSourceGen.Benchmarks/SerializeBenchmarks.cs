using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace JsonSourceGen.Benchmarks;

[MemoryDiagnoser]
public class SerializeBenchmarks
{
    private WeatherForecast _forecast = null!;
    private Order _order = null!;
    private JsonSerializerOptions _optionsWithContext = null!;

    [GlobalSetup]
    public void Setup()
    {
        _forecast = new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = 25,
            Summary = "Warm"
        };

        _order = new Order
        {
            Id = 1,
            Customer = "Alice Smith",
            OrderDate = DateTime.Now,
            ShippingAddress = new Address
            {
                Street = "123 Main St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                Country = "US"
            },
            Items =
            [
                new OrderItem { ProductId = 1, ProductName = "Widget", Quantity = 3, UnitPrice = 9.99m },
                new OrderItem { ProductId = 2, ProductName = "Gadget", Quantity = 1, UnitPrice = 24.99m },
                new OrderItem { ProductId = 3, ProductName = "Doohickey", Quantity = 5, UnitPrice = 4.50m }
            ],
            Total = 77.46m
        };

        _optionsWithContext = new JsonSerializerOptions();
        _optionsWithContext.TypeInfoResolverChain.Add(AppJsonContext.Default);
    }

    // --- Simple object: WeatherForecast ---

    [Benchmark(Description = "Simple - SourceGen")]
    public string Serialize_Simple_SourceGen()
        => JsonSerializer.Serialize(_forecast, AppJsonContext.Default.WeatherForecast);

    [Benchmark(Description = "Simple - Reflection")]
    public string Serialize_Simple_Reflection()
        => JsonSerializer.Serialize(_forecast);

    [Benchmark(Description = "Simple - SourceGen via Options")]
    public string Serialize_Simple_SourceGenViaOptions()
        => JsonSerializer.Serialize(_forecast, _optionsWithContext);

    // --- Complex object: Order ---

    [Benchmark(Description = "Complex - SourceGen")]
    public string Serialize_Complex_SourceGen()
        => JsonSerializer.Serialize(_order, AppJsonContext.Default.Order);

    [Benchmark(Description = "Complex - Reflection")]
    public string Serialize_Complex_Reflection()
        => JsonSerializer.Serialize(_order);

    [Benchmark(Description = "Complex - SourceGen via Options")]
    public string Serialize_Complex_SourceGenViaOptions()
        => JsonSerializer.Serialize(_order, _optionsWithContext);
}
