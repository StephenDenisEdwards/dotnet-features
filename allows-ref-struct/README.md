# allows ref struct

The `allows ref struct` anti-constraint, introduced in C# 13 (.NET 9+), permits generic type parameters to accept ref struct types such as `Span<T>` and `ReadOnlySpan<T>`. Previously, ref structs were implicitly excluded from all generic type parameters because they cannot satisfy the managed-object constraints the runtime assumes. With `allows ref struct`, the compiler lifts that restriction and emits the necessary IL metadata so the JIT can specialise the generic over stack-only types.

This project demonstrates:

- A generic `Sum<T>` method constrained with `INumber<T>, allows ref struct` that operates over `ReadOnlySpan<T>`.
- A generic `Print<T>` method using `allows ref struct` called with `Span<int>`, `ReadOnlySpan<char>`, and `string`.
- An interface `IBufferConsumer<T> where T : allows ref struct` implemented for `ReadOnlySpan<byte>`.

## Run

```bash
cd allows-ref-struct/console-app
dotnet run
```
