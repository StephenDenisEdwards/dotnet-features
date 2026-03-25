using System.Text.Json;
using System.Text.Json.Serialization;

var forecast = new WeatherForecast
{
    Date = DateTime.Now,
    TemperatureC = 25,
    Summary = "Warm"
};

// Source-generated serialization
var sourceGenJson = JsonSerializer.Serialize(forecast, AppJsonContext.Default.WeatherForecast);
Console.WriteLine("Source-gen serialized:");
Console.WriteLine(sourceGenJson);

// Reflection-based serialization
var reflectionJson = JsonSerializer.Serialize(forecast);
Console.WriteLine("\nReflection serialized:");
Console.WriteLine(reflectionJson);

Console.WriteLine($"\nOutputs identical: {sourceGenJson == reflectionJson}");

// Source-generated deserialization
var sourceGenDeserialized = JsonSerializer.Deserialize(sourceGenJson, AppJsonContext.Default.WeatherForecast);
Console.WriteLine($"\nSource-gen deserialized: {sourceGenDeserialized}");

// Reflection-based deserialization
var reflectionDeserialized = JsonSerializer.Deserialize<WeatherForecast>(reflectionJson);
Console.WriteLine($"Reflection deserialized: {reflectionDeserialized}");

Console.WriteLine($"\nDeserialized objects equal: {sourceGenDeserialized == reflectionDeserialized}");

// List serialization via source gen
var forecasts = new List<WeatherForecast>
{
    forecast,
    new() { Date = DateTime.Now.AddDays(1), TemperatureC = 30, Summary = "Hot" }
};

var listJson = JsonSerializer.Serialize(forecasts, AppJsonContext.Default.ListWeatherForecast);
Console.WriteLine($"\nSource-gen list serialized:\n{listJson}");

public record WeatherForecast
{
    public DateTime Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
}

[JsonSerializable(typeof(WeatherForecast))]
[JsonSerializable(typeof(List<WeatherForecast>))]
public partial class AppJsonContext : JsonSerializerContext { }
