# net11-runtime-async

This sample compares a `net10.0` baseline with a `net11.0` build that enables `runtime-async=on`.

## Projects

- [`bench/AsyncCases.Benchmarks/AsyncCases.Benchmarks.csproj`](./bench/AsyncCases.Benchmarks/AsyncCases.Benchmarks.csproj): Multi-targeting BenchmarkDotNet project (`net10.0` vs `net11.0` with `runtime-async=on`)
- [`console-net10-app/console-net10-app.csproj`](./console-net10-app/console-net10-app.csproj): console sample targeting `net10.0`
- [`console-net11-app/console-net11-app.csproj`](./console-net11-app/console-net11-app.csproj): console sample targeting `net11.0` with `runtime-async=on`

## Prerequisites

The benchmark project uses a nightly build of BenchmarkDotNet (`0.16.0-nightly.*`) because stable v0.15.8 does not recognize `net11.0`. The nightly build and its dependencies are resolved via two additional NuGet feeds configured in [`NuGet.config`](../NuGet.config) at the repository root:

- **benchmarkdotnet-nightly** — `https://www.myget.org/F/benchmarkdotnet/api/v3/index.json`
- **dotnet-tools** — `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json` (provides `Microsoft.Diagnostics.Runtime`, a transitive dependency)

Once BenchmarkDotNet v0.16.0 ships to nuget.org, these extra feeds can be removed.

## Run Benchmarks

Run from the repository root:

```powershell
dotnet run -c Release --framework net11.0 --project .\net11-runtime-async\bench\AsyncCases.Benchmarks\AsyncCases.Benchmarks.csproj -- --filter *
```

BenchmarkDotNet will automatically run each benchmark against both `net10.0` and `net11.0` and produce a single comparison table.

Notes:

- `-c Release` is required because BenchmarkDotNet rejects non-optimized builds.
- `--framework net11.0` is required because the project multi-targets. Without it, `dotnet run` will prompt you to pick a TFM. This only selects which compiled `.exe` launches BenchmarkDotNet — it does not limit which runtimes get benchmarked. BenchmarkDotNet reads the `<TargetFrameworks>` from the csproj and spawns a separate child process for each TFM, so both `net10.0` and `net11.0` are always benchmarked regardless of which framework you pass here.
- `-- --filter *` bypasses the interactive selector and runs the full benchmark set.
- BenchmarkDotNet writes output under `net11-runtime-async/bench/BenchmarkDotNet.Artifacts/`.

## Run Console Samples

Run these from the repository root:

```powershell
dotnet run --project .\net11-runtime-async\console-net10-app\console-net10-app.csproj
dotnet run --project .\net11-runtime-async\console-net11-app\console-net11-app.csproj
```

These apps are intended for side-by-side behavior and IL inspection rather than benchmark measurements.

## IL Comparison

Both console apps compile the same C# source:

```csharp
Console.WriteLine(DateTime.Now);
await Task.Delay(1000);
Console.WriteLine(DateTime.Now);
```

The IL they produce is dramatically different.

### net10.0 — compiler-generated async state machine

The compiler generates a full async state machine: a nested struct `<<Main>$>d__0` implementing `IAsyncStateMachine` with a `<>1__state` field to track where execution suspended, an `AsyncTaskMethodBuilder` to manage the Task lifecycle, a `TaskAwaiter` field to store the pending awaiter, and a `MoveNext()` method with branching logic to resume after the `await`. Total: ~250 bytes of IL, a nested type, 3 fields, and 2 interface methods.

```il
.class private auto ansi beforefieldinit Program
    extends [System.Runtime]System.Object
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    // Nested Types
    .class nested private auto ansi sealed beforefieldinit '<<Main>$>d__0'
        extends [System.Runtime]System.ValueType
        implements [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        // Fields
        .field public int32 '<>1__state'
        .field public valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
        .field private valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'

        // Methods
        .method private final hidebysig newslot virtual
            instance void MoveNext () cil managed
        {
            .override method instance void [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext()
            .maxstack 3
            .locals init (
                [0] int32,
                [1] valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter,
                [2] class [System.Runtime]System.Exception
            )

            IL_0000: ldarg.0
            IL_0001: ldfld int32 Program/'<<Main>$>d__0'::'<>1__state'
            IL_0006: stloc.0
            .try
            {
                IL_0007: ldloc.0
                IL_0008: brfalse.s IL_0052

                IL_000a: call valuetype [System.Runtime]System.DateTime [System.Runtime]System.DateTime::get_Now()
                IL_000f: box [System.Runtime]System.DateTime
                IL_0014: call void [System.Console]System.Console::WriteLine(object)
                IL_0019: ldc.i4 1000
                IL_001e: call class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::Delay(int32)
                IL_0023: callvirt instance valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter [System.Runtime]System.Threading.Tasks.Task::GetAwaiter()
                IL_0028: stloc.1
                IL_0029: ldloca.s 1
                IL_002b: call instance bool [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
                IL_0030: brtrue.s IL_006e

                IL_0032: ldarg.0
                IL_0033: ldc.i4.0
                IL_0034: dup
                IL_0035: stloc.0
                IL_0036: stfld int32 Program/'<<Main>$>d__0'::'<>1__state'
                IL_003b: ldarg.0
                IL_003c: ldloc.1
                IL_003d: stfld valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter Program/'<<Main>$>d__0'::'<>u__1'
                IL_0042: ldarg.0
                IL_0043: ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program/'<<Main>$>d__0'::'<>t__builder'
                IL_0048: ldloca.s 1
                IL_004a: ldarg.0
                IL_004b: call instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter, valuetype Program/'<<Main>$>d__0'>(!!0&, !!1&)
                IL_0050: leave.s IL_00b0

                IL_0052: ldarg.0
                IL_0053: ldfld valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter Program/'<<Main>$>d__0'::'<>u__1'
                IL_0058: stloc.1
                IL_0059: ldarg.0
                IL_005a: ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter Program/'<<Main>$>d__0'::'<>u__1'
                IL_005f: initobj [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter
                IL_0065: ldarg.0
                IL_0066: ldc.i4.m1
                IL_0067: dup
                IL_0068: stloc.0
                IL_0069: stfld int32 Program/'<<Main>$>d__0'::'<>1__state'

                IL_006e: ldloca.s 1
                IL_0070: call instance void [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
                IL_0075: call valuetype [System.Runtime]System.DateTime [System.Runtime]System.DateTime::get_Now()
                IL_007a: box [System.Runtime]System.DateTime
                IL_007f: call void [System.Console]System.Console::WriteLine(object)
                IL_0084: leave.s IL_009d
            } // end .try
            catch [System.Runtime]System.Exception
            {
                IL_0086: stloc.2
                IL_0087: ldarg.0
                IL_0088: ldc.i4.s -2
                IL_008a: stfld int32 Program/'<<Main>$>d__0'::'<>1__state'
                IL_008f: ldarg.0
                IL_0090: ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program/'<<Main>$>d__0'::'<>t__builder'
                IL_0095: ldloc.2
                IL_0096: call instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [System.Runtime]System.Exception)
                IL_009b: leave.s IL_00b0
            } // end handler

            IL_009d: ldarg.0
            IL_009e: ldc.i4.s -2
            IL_00a0: stfld int32 Program/'<<Main>$>d__0'::'<>1__state'
            IL_00a5: ldarg.0
            IL_00a6: ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program/'<<Main>$>d__0'::'<>t__builder'
            IL_00ab: call instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()

            IL_00b0: ret
        } // end of method '<<Main>$>d__0'::MoveNext

        .method private final hidebysig newslot virtual
            instance void SetStateMachine (
                class [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine
            ) cil managed
        {
            .override method instance void [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine(class [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine)
            IL_0000: ldarg.0
            IL_0001: ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program/'<<Main>$>d__0'::'<>t__builder'
            IL_0006: ldarg.1
            IL_0007: call instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [System.Runtime]System.Runtime.CompilerServices.IAsyncStateMachine)
            IL_000c: ret
        } // end of method '<<Main>$>d__0'::SetStateMachine

    } // end of class <<Main>$>d__0

    // Methods
    .method private hidebysig static
        class [System.Runtime]System.Threading.Tasks.Task '<Main>$' (
            string[] args
        ) cil managed
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [System.Runtime]System.Type) = (
            01 00 15 50 72 6f 67 72 61 6d 2b 3c 3c 4d 61 69
            6e 3e 24 3e 64 5f 5f 30 00 00
        )
        .maxstack 2
        .locals init (
            [0] valuetype Program/'<<Main>$>d__0'
        )

        IL_0000: ldloca.s 0
        IL_0002: call valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
        IL_0007: stfld valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program/'<<Main>$>d__0'::'<>t__builder'
        IL_000c: ldloca.s 0
        IL_000e: ldc.i4.m1
        IL_000f: stfld int32 Program/'<<Main>$>d__0'::'<>1__state'
        IL_0014: ldloca.s 0
        IL_0016: ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program/'<<Main>$>d__0'::'<>t__builder'
        IL_001b: ldloca.s 0
        IL_001d: call instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype Program/'<<Main>$>d__0'>(!!0&)
        IL_0022: ldloca.s 0
        IL_0024: ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder Program/'<<Main>$>d__0'::'<>t__builder'
        IL_0029: call instance class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
        IL_002e: ret
    } // end of method Program::'<Main>$'
} // end of class Program
```

### net11.0 with `runtime-async=on` — no state machine

No state machine at all. The `await` compiles to a simple `call void AsyncHelpers::Await(Task)`. The runtime/JIT handles all suspension and resumption internally — no compiler-generated state machine, no builder, no nested type. The method reads like straight-line synchronous code: 46 bytes of IL total.

```il
.class private auto ansi beforefieldinit Program
    extends [System.Runtime]System.Object
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )
    // Methods
    .method private hidebysig static
        class [System.Runtime]System.Threading.Tasks.Task '<Main>$' (
            string[] args
        ) cil managed flag(2000)
    {
        .maxstack 8

        IL_0000: call valuetype [System.Runtime]System.DateTime [System.Runtime]System.DateTime::get_Now()
        IL_0005: box [System.Runtime]System.DateTime
        IL_000a: call void [System.Console]System.Console::WriteLine(object)
        IL_000f: ldc.i4 1000
        IL_0014: call class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::Delay(int32)
        IL_0019: call void [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(class [System.Runtime]System.Threading.Tasks.Task)
        IL_001e: call valuetype [System.Runtime]System.DateTime [System.Runtime]System.DateTime::get_Now()
        IL_0023: box [System.Runtime]System.DateTime
        IL_0028: call void [System.Console]System.Console::WriteLine(object)
        IL_002d: ret
    } // end of method Program::'<Main>$'
} // end of class Program
```

### What this means

The `runtime-async` feature moves async machinery from **compiler-generated IL** to the **runtime/JIT**. This results in far less IL, fewer allocations, and the potential for the JIT to optimize suspension paths that the compiler could never see. The IL for the net11.0 build looks almost identical to what a synchronous version of the code would produce — the only difference is the `AsyncHelpers::Await()` call where `await` appears in the source.

### Debugging impact

The IL difference has a direct effect on the debugging experience. With traditional async (net10.0), when you set a breakpoint on `await Task.Delay(1000)`, the debugger is actually stepping through a compiler-generated `MoveNext()` method inside a hidden struct called `<<Main>$>d__0`. Your local variables have been hoisted into fields like `<>1__state` and `<>u__1`. The call stack is full of infrastructure frames.

With runtime-async (net11.0), your method is just your method — `Console.WriteLine`, `Task.Delay`, `AsyncHelpers.Await`, `Console.WriteLine`, `ret`. When you hit a breakpoint, you're in the actual `<Main>$` method. Your locals are real locals on the stack, not fields on a hidden struct.

For a 3-method async call chain, the difference is stark:

**net10.0 stack trace (~13 frames):**
```
Program+<<Main>$>d__0.MoveNext()
AsyncTaskMethodBuilder.SetResult()
AsyncTaskMethodBuilder<T>.AwaitUnsafeOnCompleted(...)
ServiceA+<GetDataAsync>d__1.MoveNext()
AsyncTaskMethodBuilder.SetResult()
AsyncTaskMethodBuilder<T>.AwaitUnsafeOnCompleted(...)
ServiceB+<FetchAsync>d__2.MoveNext()
...runtime plumbing...
```

**net11.0 stack trace (~5 frames):**
```
Program.<Main>$(string[])
ServiceA.GetDataAsync()
ServiceB.FetchAsync()
...
```

The state machine frames, builder frames, and `MoveNext` indirection are all gone. What you see in the debugger matches what you wrote in the editor.

## Benchmark Results

Tested on BenchmarkDotNet v0.16.0-nightly.20260324.479, Windows 11, 11th Gen Intel Core i7-11370H 3.30GHz, 4 physical cores, 31.84 GB RAM.

```
| Method                  | Runtime   | Mean       | Allocated |
|------------------------ |---------- |-----------:|----------:|
| SynchronousBaseline     | .NET 10.0 |   1.076 ns |         - |
| CompletedTask           | .NET 10.0 |   2.025 ns |         - |
| CompletedValueTask      | .NET 10.0 |   6.947 ns |         - |
| AwaitCompletedTask      | .NET 10.0 |   6.689 ns |         - |
| AwaitCompletedValueTask | .NET 10.0 |  12.120 ns |         - |
| AwaitFromResult         | .NET 10.0 |  19.264 ns |     144 B |
| AwaitYield              | .NET 10.0 | 179.382 ns |     144 B |
|                         |           |            |           |
| SynchronousBaseline     | .NET 11.0 |   1.261 ns |         - |
| CompletedTask           | .NET 11.0 |   2.570 ns |         - |
| CompletedValueTask      | .NET 11.0 |   2.809 ns |         - |
| AwaitCompletedTask      | .NET 11.0 |   9.801 ns |         - |
| AwaitCompletedValueTask | .NET 11.0 |  11.476 ns |         - |
| AwaitFromResult         | .NET 11.0 |  23.449 ns |     144 B |
| AwaitYield              | .NET 11.0 | 276.245 ns |     424 B |
```

### Analysis

These results are from .NET 11.0 Preview 2 — the `runtime-async` feature is still early and not fully optimized.

**Where runtime-async wins:**

- `CompletedValueTask` — **2.5x faster** (6.95 ns → 2.81 ns). This is the synchronous-completion fast path where the runtime can skip the state machine entirely.
- `AwaitCompletedValueTask` — modest improvement (12.12 ns → 11.48 ns).

**Where runtime-async is slower (for now):**

- `SynchronousBaseline` is ~17% slower on .NET 11.0 (1.08 ns → 1.26 ns). This is a pure `return 42` with no async — the difference reflects a general baseline overhead in this early preview, not a runtime-async issue.
- `AwaitCompletedTask` is slower (6.69 ns → 9.80 ns).
- `AwaitFromResult` is slower (19.26 ns → 23.45 ns) and allocates the same 144B — no allocation improvement yet.
- `AwaitYield` is **54% slower** (179 ns → 276 ns) **and allocates 3x more** (144B → 424B). This is the only benchmark that truly suspends, and the runtime's internal state-saving mechanism currently has more overhead than the compiler-generated state machine.

**Takeaway:** In this early preview, runtime-async shows clear benefits for the synchronous-completion path (already-completed ValueTask), but the actual suspension path is not yet optimized. The extra allocations in `AwaitYield` (424B vs 144B) confirm the runtime is creating larger internal structures than the compiler's `AsyncTaskMethodBuilder` + state machine. This is expected to improve as .NET 11 matures toward release.

### Why these results are expected (for now)

These results are consistent with what Microsoft and the community are observing. The key context:

**Core runtime libraries are not yet compiled with runtime-async.** In Preview 1 and 2, `System.Net.Http`, Kestrel, `System.Threading.Tasks`, and other framework code still use traditional compiler-generated async state machines. Microsoft explicitly acknowledges this and states it "is expected to change in upcoming previews." When our `AwaitYield` benchmark calls `Task.Yield()`, it crosses a boundary between runtime-async user code and traditional-async framework code, incurring a penalty.

**Community benchmarks show the same pattern:**

| Scenario | Change |
|---|---|
| Synthetic `Task.FromResult` chain (no framework boundary) | ~33% faster, ~42% less memory |
| Nested async calls (no framework boundary) | ~66% faster, ~60% less memory |
| 10,000 concurrent ops (hits framework code) | **41% slower** |
| High-pressure parallel (hits framework code) | **23% slower** |

Our `CompletedValueTask` (2.5x faster) aligns with the "synthetic fast path" wins. Our `AwaitYield` (54% slower, 3x more allocations) aligns with the "hits framework code" regressions.

**Microsoft's official messaging focuses on debuggability, not throughput.** The [.NET 11 Preview 1](https://devblogs.microsoft.com/dotnet/dotnet-11-preview-1/) and [Preview 2](https://devblogs.microsoft.com/dotnet/dotnet-11-preview-2/) announcements emphasize cleaner stack traces (13 frames → 5 for a 3-method async chain), better debugger stepping through `await` boundaries, and reduced NativeAOT binary sizes. They are notably restrained about raw performance claims. Stephen Toub has not yet published a "Performance Improvements in .NET 11" blog post.

**The full benefits are expected once core libraries are recompiled** with runtime-async in later previews toward GA (November 2026). At that point, the boundary-crossing penalty disappears and the runtime can optimize the entire async call chain end-to-end.

### References

- [What's new in .NET 11 runtime (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime)
- [.NET 11 Preview 1 announcement](https://devblogs.microsoft.com/dotnet/dotnet-11-preview-1/)
- [.NET 11 Preview 2 announcement](https://devblogs.microsoft.com/dotnet/dotnet-11-preview-2/)
- [Runtime-Async design spec](https://github.com/dotnet/runtime/blob/main/docs/design/specs/runtime-async.md)
- [Runtime-Async tracking issue #109632](https://github.com/dotnet/runtime/issues/109632)
- [.NET 9 Runtime Async Experiment #94620](https://github.com/dotnet/runtime/issues/94620) — concluded runtime-async was "at least as good as compiler-async" under controlled conditions
- [Steven Giesel: "New runtime async is hitting .NET 11"](https://steven-giesel.com/blogPost/1fb10ed2-df84-4080-b660-72c04a4cc674) — community benchmarks showing 33% faster / 42% less memory in synthetic cases
