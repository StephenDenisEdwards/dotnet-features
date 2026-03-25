# native-aot

Compares JIT and NativeAOT compilation for the same application. Measures startup time, working set, and throughput.

## Build and Run

From the repository root:

### JIT (default)
```powershell
dotnet run -c Release --project .\native-aot\console-app\console-app.csproj
```

### NativeAOT
```powershell
dotnet publish -c Release --project .\native-aot\console-app\console-app.csproj /p:PublishAot=true -o .\native-aot\publish-aot
.\native-aot\publish-aot\console-app.exe
```

Compare the output of both runs, plus the binary sizes:
- JIT: `native-aot\console-app\bin\Release\net10.0\console-app.dll`
- AOT: `native-aot\publish-aot\console-app.exe`
