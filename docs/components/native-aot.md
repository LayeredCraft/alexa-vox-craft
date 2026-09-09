# Native AOT Support

AlexaVoxCraft supports publishing consumer skills with `PublishAot=true`. Every package AlexaVoxCraft ships its own generated serialization metadata for; your own request/response types and any types you exchange with SMAPI need one extra step, described below.

## Quick summary

- No code changes are required to keep running under the JIT (`dotnet run`, ordinary `dotnet publish` without `PublishAot`).
- Publishing with `PublishAot=true` works out of the box for every type AlexaVoxCraft itself owns (core requests/responses, APL components, In-Skill Purchasing directives, SMAPI models).
- For your **own** types - a custom session-state POCO, a custom SMAPI `TRequest`/`TResponse` for `AlexaSkillInvocationClient.InvokeAsync`, anything you serialize through `AlexaJsonOptions.DefaultOptions` - register your own source-generated `JsonSerializerContext` once at startup.

## Registering your own types

Declare a `JsonSerializerContext` for your types using System.Text.Json's built-in source generator, and register it once:

```csharp
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Serialization;

public class GameState
{
    public int Score { get; set; }
    public string? Level { get; set; }
}

[JsonSerializable(typeof(GameState))]
internal partial class MySkillJsonContext : JsonSerializerContext
{
}

// At startup, once:
AlexaJsonOptions.RegisterTypeInfoResolver(MySkillJsonContext.Default);
```

After this call, `GameState` (and every other type declared with `[JsonSerializable]` on `MySkillJsonContext`) works everywhere AlexaVoxCraft uses `AlexaJsonOptions.DefaultOptions` - `JsonAttributeBag.Set<T>`/`Get<T>` for session/persistent attributes, `AlexaSkillInvocationClient.InvokeAsync<TRequest, TResponse>` for your own request/response bodies, and any `AlexaVoxCraft.Http`-based client's default serialization path.

Registration order doesn't matter relative to when clients or the mediator were constructed - a registration made after a client already exists is still picked up before that client's next actual serialize/deserialize call.

## JIT fallback vs. Native AOT requirement

When reflection is available (any normal `dotnet run`/`dotnet publish` without `PublishAot`), AlexaVoxCraft falls back to reflection-based serialization for a type it doesn't otherwise have metadata for - registering your own context is optional there, purely a performance/startup-time optimization.

Under `PublishAot=true`, that reflection fallback is compiled out entirely. A type with no registered metadata fails immediately and clearly:

```
System.NotSupportedException: ... has no registered converter/JsonTypeInfo ...
```

rather than a confusing runtime failure deep inside serialization. If you see this under a Native AOT build, it means a type you're serializing needs to be added to your own `JsonSerializerContext` and registered via `RegisterTypeInfoResolver`.

## Your own `JsonSerializerOptions`

If you construct an `AlexaVoxCraft.Http`-based client (or any SMAPI client) with your own explicit `JsonSerializerOptions`, AlexaVoxCraft never mutates or replaces it - you own that configuration completely, including its `TypeInfoResolver`. The guidance above applies only to AlexaVoxCraft's own default options paths.

## Verifying your own skill

The same shape this repository uses to validate itself (`test/AlexaVoxCraft.NativeAot.ValidationApp` in this repository's source) is a reasonable template: a small console entry point exercising your handlers and serialization paths, published with:

```xml
<PublishAot>true</PublishAot>
<InvariantGlobalization>true</InvariantGlobalization>
<EnableTrimAnalyzer>true</EnableTrimAnalyzer>
<EnableAotAnalyzer>true</EnableAotAnalyzer>
<JsonSerializerIsReflectionEnabledByDefault>false</JsonSerializerIsReflectionEnabledByDefault>
```

then published (`dotnet publish -r linux-x64 -p:PublishAot=true`, matching AWS Lambda's `provided.al2023` runtime) and actually executed - not just built - to confirm your own handlers and types work end to end.

### Source-tree note for interceptor-based DI registration

If you reference AlexaVoxCraft.MediatR via a source-tree `ProjectReference` (rather than a published NuGet package), also add a direct `ProjectReference` to `AlexaVoxCraft.MediatR.Generators` with `OutputItemType="Analyzer"` so its interceptor participates in your build - a plain `ProjectReference` to `AlexaVoxCraft.MediatR` alone does not pull in its interceptor generator (that only happens automatically for a packaged NuGet consumer). This does not apply to consumers installing AlexaVoxCraft.MediatR from NuGet.
