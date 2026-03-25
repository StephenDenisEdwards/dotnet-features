using System.Diagnostics;
using System.Text.Json;

var sw = Stopwatch.StartNew();

// JSON serialization workload
var data = new WeatherForecast
{
    Date = DateTime.Now,
    TemperatureC = 25,
    Summary = "Warm"
};

string json = "";
for (int i = 0; i < 1000; i++)
{
    json = JsonSerializer.Serialize(data);
    _ = JsonSerializer.Deserialize<WeatherForecast>(json);
}

sw.Stop();

Console.WriteLine($"Startup + 1000 serialize/deserialize cycles: {sw.ElapsedMilliseconds}ms");
Console.WriteLine($"Working set: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024}MB");
Console.WriteLine($"Last JSON: {json}");

record WeatherForecast
{
    public DateTime Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
}
