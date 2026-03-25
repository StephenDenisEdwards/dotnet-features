using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    sealed class InterceptsLocationAttribute(string filePath, int line, int column) : Attribute;
}

namespace ConsoleApp.Interceptors
{
    public static class GreeterInterceptor
    {
        [InterceptsLocation(@"C:\Users\steph\source\repos\dotnet-features\interceptors\console-app\Program.cs", line: 2, column: 27)]
        public static string SayHello_Intercepted(this Greeter greeter, string name)
            => $"Intercepted: Greetings, {name}!";
    }
}
