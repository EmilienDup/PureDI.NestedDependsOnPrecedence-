# Binding precedence is reversed through a chain of `DependsOn` setups (the base setup wins over the setup that overrides it)

**Pure.DI 2.5.6**, .NET SDK 10.0.402, macOS. Repro project attached (`dotnet run`).

With `DependsOn("Base", "Mid")` listed directly, the later setup wins, as documented. When the same two setups are reached through a chain, `Composition -> Mid -> Base` where `Mid` itself depends on `Base` and re-binds a contract, the binding from `Base` is used instead of the one from `Mid`.

## Minimal code

```csharp
using Pure.DI;

Console.WriteLine($"Direct: {new Direct().Greeting.Text}");
Console.WriteLine($"Nested: {new Nested().Greeting.Text}");

public interface IGreeting { string Text { get; } }
public sealed record Greeting(string Text) : IGreeting;

static class Setups
{
    private static void Setup()
    {
        DI.Setup("Base", CompositionKind.Internal)
            .Bind<IGreeting>().To(_ => new Greeting("base"));

        // Mid builds on Base and replaces the greeting
        DI.Setup("Mid", CompositionKind.Internal)
            .DependsOn("Base")
            .Bind<IGreeting>().To(_ => new Greeting("mid"));

        // the two setups listed directly
        DI.Setup(nameof(Direct))
            .DependsOn("Base", "Mid")
            .Root<IGreeting>("Greeting");

        // through the chain Nested -> Mid -> Base
        DI.Setup(nameof(Nested))
            .DependsOn("Mid")
            .Root<IGreeting>("Greeting");
    }
}
```

## Expected

```
Direct: mid
Nested: mid
```

`Mid` is the setup that knows about `Base` and deliberately replaces its binding, so a composition built on `Mid` should get `Mid`'s binding.

## Actual

```
Direct: mid
Nested: base
```

The build only reports the usual overridden-binding warning; nothing indicates that the chain resolves the other way round. A layered setup (base library setup, a layer setup refining it, a composition on top) silently gets the base bindings.
