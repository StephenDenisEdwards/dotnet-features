using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace JsonSourceGen.Benchmarks;

[MemoryDiagnoser]
public class DeserializeBenchmarks
{
    private string _forecastJson = null!;
    private string _orderJson = null!;
    private JsonSerializerOptions _optionsWithContext = null!;

    [GlobalSetup]
    public void Setup()
    {
        var forecast = new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = 25,
            Summary = "Warm"
        };

        var order = new Order
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

        _forecastJson = JsonSerializer.Serialize(forecast, AppJsonContext.Default.WeatherForecast);
        _orderJson = JsonSerializer.Serialize(order, AppJsonContext.Default.Order);

        _optionsWithContext = new JsonSerializerOptions();
        _optionsWithContext.TypeInfoResolverChain.Add(AppJsonContext.Default);
    }

    // --- Simple object: WeatherForecast ---

    [Benchmark(Description = "Simple - SourceGen")]
    public WeatherForecast? Deserialize_Simple_SourceGen()
        => JsonSerializer.Deserialize(_forecastJson, AppJsonContext.Default.WeatherForecast);

    [Benchmark(Description = "Simple - Reflection")]
    public WeatherForecast? Deserialize_Simple_Reflection()
        => JsonSerializer.Deserialize<WeatherForecast>(_forecastJson);

    [Benchmark(Description = "Simple - SourceGen via Options")]
    public WeatherForecast? Deserialize_Simple_SourceGenViaOptions()
        => JsonSerializer.Deserialize<WeatherForecast>(_forecastJson, _optionsWithContext);

    // --- Complex object: Order ---

    [Benchmark(Description = "Complex - SourceGen")]
    public Order? Deserialize_Complex_SourceGen()
        => JsonSerializer.Deserialize(_orderJson, AppJsonContext.Default.Order);

    [Benchmark(Description = "Complex - Reflection")]
    public Order? Deserialize_Complex_Reflection()
        => JsonSerializer.Deserialize<Order>(_orderJson);

    [Benchmark(Description = "Complex - SourceGen via Options")]
    public Order? Deserialize_Complex_SourceGenViaOptions()
        => JsonSerializer.Deserialize<Order>(_orderJson, _optionsWithContext);
}
