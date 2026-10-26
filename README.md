# Pure.DI: binding precedence reversed through nested `DependsOn`

Minimal repro for Pure.DI 2.5.6: a setup that depends on a base setup and re-binds a contract loses to the base setup when a composition reaches it through `DependsOn("Mid")`.

[`ISSUE.md`](ISSUE.md) is the text to paste into a Pure.DI GitHub issue.

```
dotnet run
```

prints `Direct: mid` and `Nested: base`; both are expected to be `mid`.
