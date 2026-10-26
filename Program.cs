using Pure.DI;

Console.WriteLine($"Direct: {new Direct().Greeting.Text}");   // expected: mid
Console.WriteLine($"Nested: {new Nested().Greeting.Text}");   // expected: mid

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

        // same two setups listed directly: later one wins, as documented
        DI.Setup(nameof(Direct))
            .DependsOn("Base", "Mid")
            .Root<IGreeting>("Greeting");

        // through the chain Nested -> Mid -> Base
        DI.Setup(nameof(Nested))
            .DependsOn("Mid")
            .Root<IGreeting>("Greeting");
    }
}
