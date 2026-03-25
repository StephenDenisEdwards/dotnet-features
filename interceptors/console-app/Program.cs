var greeter = new Greeter();
Console.WriteLine(greeter.SayHello("World"));

public class Greeter
{
    public string SayHello(string name) => $"Hello, {name}!";
}
