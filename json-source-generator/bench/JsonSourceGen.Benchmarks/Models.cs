namespace JsonSourceGen.Benchmarks;

public record WeatherForecast
{
    public DateTime Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
}

public class Order
{
    public int Id { get; set; }
    public string Customer { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public Address ShippingAddress { get; set; } = new();
    public List<OrderItem> Items { get; set; } = [];
    public decimal Total { get; set; }
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
