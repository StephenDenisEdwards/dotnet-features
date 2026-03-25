// Simulate tasks completing at different times
var tasks = Enumerable.Range(1, 5)
    .Select(async i =>
    {
        await Task.Delay(Random.Shared.Next(100, 1000));
        return i;
    })
    .ToList();

Console.WriteLine("=== Task.WhenEach (process as completed) ===");
var tasks1 = tasks.Select(t => t).ToList(); // can't reuse
// Actually we need fresh tasks each time
var whenEachTasks = Enumerable.Range(1, 5)
    .Select(async i =>
    {
        await Task.Delay(Random.Shared.Next(100, 1000));
        return i;
    })
    .ToList();

await foreach (var task in Task.WhenEach(whenEachTasks))
{
    Console.WriteLine($"  Completed: {task.Result} at {DateTime.Now:HH:mm:ss.fff}");
}

Console.WriteLine("\n=== Task.WhenAll (wait for all, then process) ===");
var whenAllTasks = Enumerable.Range(1, 5)
    .Select(async i =>
    {
        await Task.Delay(Random.Shared.Next(100, 1000));
        return i;
    })
    .ToList();

var results = await Task.WhenAll(whenAllTasks);
foreach (var result in results)
{
    Console.WriteLine($"  Result: {result} at {DateTime.Now:HH:mm:ss.fff}");
}
