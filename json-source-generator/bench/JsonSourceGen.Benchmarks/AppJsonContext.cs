using System.Text.Json.Serialization;

namespace JsonSourceGen.Benchmarks;

[JsonSerializable(typeof(WeatherForecast))]
[JsonSerializable(typeof(List<WeatherForecast>))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(List<Order>))]
[JsonSerializable(typeof(Address))]
[JsonSerializable(typeof(OrderItem))]
[JsonSerializable(typeof(List<OrderItem>))]
public partial class AppJsonContext : JsonSerializerContext { }
