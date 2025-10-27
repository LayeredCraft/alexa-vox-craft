# Source-Generated Dependency Injection

AlexaVoxCraft automatically generates dependency injection registration code at compile time using C# interceptors, eliminating runtime reflection and improving startup performance.

> 🎯 **Trivia Skill Examples**: All code examples demonstrate building a **trivia game skill** with automatic handler registration.

## Features

- **⚡ Compile-Time Registration**: Zero runtime reflection or assembly scanning
- **🎯 Type-Safe**: Compile-time validation of handler implementations
- **🚀 Faster Startup**: No reflection overhead during Lambda cold starts
- **🔧 Customizable**: Control registration with attributes
- **📦 Automatic**: Works out of the box with no configuration

## Requirements

⚠️ **SDK Version Required**: To use source-generated dependency injection with interceptors, you must use at least version **8.0.400** of the .NET SDK. This ships with **Visual Studio 2022 version 17.11** or higher.

Check your SDK version:
```bash
dotnet --version
# Should show 8.0.400 or higher
```

## How It Works

The source generator uses C# 12 interceptors to replace calls to `AddSkillMediator()` with compile-time generated registration code:

```csharp
// Your code - no assembly scanning needed!
services.AddSkillMediator(context.Configuration);

// Generated at compile time - intercepts the call above
internal static IServiceCollection AddSkillMediator(
    this IServiceCollection services,
    IConfiguration configuration,
    Action<SkillServiceConfiguration>? settingsAction = null)
{
    // Auto-generated registrations
    services.AddTransient<IRequestHandler<LaunchRequest>, LaunchRequestHandler>();
    services.AddTransient<IRequestHandler<IntentRequest>, AnswerHandler>();
    services.AddTransient<IRequestHandler<UserEventRequest>, AnswerHandler>();
    services.TryAddTransient<IDefaultRequestHandler, DefaultHandler>();
    services.AddTransient<IExceptionHandler, ErrorHandler>();
    services.AddTransient<IRequestInterceptor, LocalizationRequestInterceptor>();

    return services;
}
```

### Benefits

1. **No Runtime Reflection**: All handler discovery happens at compile time
2. **Faster Cold Starts**: Eliminates assembly scanning during Lambda initialization
3. **Compile-Time Validation**: Errors caught during build, not at runtime
4. **IDE Support**: Full IntelliSense and navigation for generated code

## Default Behavior

Source generation is **enabled by default**. Simply install the `AlexaVoxCraft.MediatR` package and call `AddSkillMediator()`:

```csharp
// In your AlexaSkillFunction
protected override void Init(IHostBuilder builder)
{
    builder
        .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
        .ConfigureServices((context, services) =>
        {
            // This call is automatically intercepted at compile time
            // No assembly scanning - all registration is generated!
            services.AddSkillMediator(context.Configuration);
        });
}
```

The generator discovers and registers:
- `IRequestHandler<T>` implementations
- `IDefaultRequestHandler` implementations
- `IPipelineBehavior` implementations
- `IExceptionHandler` implementations
- `IRequestInterceptor` implementations
- `IResponseInterceptor` implementations
- `IPersistenceAdapter` implementations

## Customizing Registration

### The AlexaHandler Attribute

Control how handlers are registered using the `[AlexaHandler]` attribute:

```csharp
using AlexaVoxCraft.MediatR.Annotations;

[AlexaHandler(Lifetime = ServiceLifetime.Scoped, Order = 10, Exclude = false)]
public class MyHandler : IRequestHandler<LaunchRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return Task.FromResult(input.RequestEnvelope.Request is LaunchRequest);
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return await input.ResponseBuilder
            .Speak("Welcome!")
            .GetResponse(cancellationToken);
    }
}
```

### Lifetime Property

Controls the service lifetime for dependency injection:

> **Default Lifetime**: When the `Lifetime` property is not specified, handlers are registered as `Transient` (a new instance is created for each request). This is the recommended default for stateless request handlers.

```csharp
// Transient (default when Lifetime not specified) - new instance per request
[AlexaHandler(Lifetime = ServiceLifetime.Transient)]
public class TransientHandler : IRequestHandler<LaunchRequest> { }

// Scoped - one instance per Lambda invocation
[AlexaHandler(Lifetime = ServiceLifetime.Scoped)]
public class ScopedHandler : IRequestHandler<IntentRequest> { }

// Singleton - one instance for the lifetime of the Lambda container
[AlexaHandler(Lifetime = ServiceLifetime.Singleton)]
public class SingletonHandler : IPipelineBehavior { }
```

**Best Practices:**
- Use `Transient` for stateless handlers (default)
- Use `Scoped` for handlers that share state within a single request
- Use `Singleton` for expensive-to-create services or caches
- **Warning**: Be careful with `Singleton` - ensure thread-safety

### Order Property

Controls execution order for handlers and pipeline behaviors:

```csharp
// Lower numbers execute first
[AlexaHandler(Order = 1)]
public class AuthenticationInterceptor : IRequestInterceptor
{
    // Runs first - validates authentication
}

[AlexaHandler(Order = 5)]
public class LoggingInterceptor : IRequestInterceptor
{
    // Runs second - logs authenticated requests
}

[AlexaHandler(Order = 10)]
public class LocalizationInterceptor : IRequestInterceptor
{
    // Runs third - sets up localization
}
```

**Ordering Pipeline Behaviors:**

```csharp
// Behavior 1: Validation (Order = 0 - default)
[AlexaHandler]
public class ValidationBehavior : IPipelineBehavior
{
    public async Task<SkillResponse> Handle(
        SkillRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<SkillResponse> next)
    {
        // Validate request
        if (!IsValid(request))
            throw new ValidationException();

        return await next();
    }
}

// Behavior 2: Logging (Order = 5)
[AlexaHandler(Order = 5)]
public class LoggingBehavior : IPipelineBehavior
{
    public async Task<SkillResponse> Handle(
        SkillRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<SkillResponse> next)
    {
        _logger.LogInformation("Processing request");
        var response = await next();
        _logger.LogInformation("Request processed");
        return response;
    }
}

// Behavior 3: Performance Monitoring (Order = 10)
[AlexaHandler(Order = 10)]
public class PerformanceBehavior : IPipelineBehavior
{
    public async Task<SkillResponse> Handle(
        SkillRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<SkillResponse> next)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        _logger.LogInformation("Processed in {elapsed}ms", sw.ElapsedMilliseconds);
        return response;
    }
}
```

**Execution Order**: Validation → Logging → Performance Monitoring → Handler → Performance Monitoring → Logging → Validation

### Exclude Property

Skip automatic registration for specific handlers:

```csharp
// This handler will NOT be automatically registered
[AlexaHandler(Exclude = true)]
public class ManuallyRegisteredHandler : IRequestHandler<LaunchRequest>
{
    // Implementation
}

// Register it manually if needed
services.AddScoped<IRequestHandler<LaunchRequest>, ManuallyRegisteredHandler>();
```

**Use Cases for Exclude:**
- Testing - register mock handlers manually
- Conditional registration - register based on environment
- Custom registration logic - need special configuration

## Multiple Interface Implementations

Handlers can implement multiple `IRequestHandler<T>` interfaces and all will be registered:

```csharp
// Handles both IntentRequest and UserEventRequest
public class AnswerHandler :
    IRequestHandler<IntentRequest>,
    IRequestHandler<UserEventRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            input.RequestEnvelope.Request is UserEventRequest ||
            input.RequestEnvelope.Request is IntentRequest intent &&
            (intent.Intent.Name == "AnswerIntent" || intent.Intent.Name == "DontKnowIntent"));
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        // Handle both request types
        var answer = GetAnswer(input.RequestEnvelope.Request);
        // Process answer...
    }
}

// Generated registration
services.AddTransient<IRequestHandler<IntentRequest>, AnswerHandler>();
services.AddTransient<IRequestHandler<UserEventRequest>, AnswerHandler>();
```

Similarly, a class can implement both `IRequestInterceptor` and `IResponseInterceptor`:

```csharp
public class FullCycleInterceptor :
    IRequestInterceptor,
    IResponseInterceptor
{
    public Task Process(IHandlerInput input, CancellationToken cancellationToken)
    {
        // Pre-process request
        return Task.CompletedTask;
    }

    public Task Process(IHandlerInput input, SkillResponse? output, CancellationToken cancellationToken)
    {
        // Post-process response
        return Task.CompletedTask;
    }
}

// Both interfaces registered
services.AddTransient<IRequestInterceptor, FullCycleInterceptor>();
services.AddTransient<IResponseInterceptor, FullCycleInterceptor>();
```

## Disabling Source Generation

If you need to disable the source generator and use runtime registration instead:

```xml
<!-- In your .csproj file -->
<PropertyGroup>
  <EnableMediatRGeneratorInterceptor>false</EnableMediatRGeneratorInterceptor>
</PropertyGroup>
```

When disabled, `AddSkillMediator()` falls back to runtime reflection-based registration and you **must** specify the assembly to scan:

```csharp
// With source generation DISABLED - requires assembly scanning configuration
services.AddSkillMediator(context.Configuration, cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());  // ⚠️ Only needed when source generation is disabled
```

**When to Disable:**
- Debugging generator issues
- Using .NET SDK version < 8.0.400
- Need dynamic handler registration at runtime
- Testing custom registration scenarios

## Default Handler Registration

The generator uses `TryAddTransient` for `IDefaultRequestHandler` to allow manual override:

```csharp
// Auto-registered as default handler
public class FallbackHandler : IDefaultRequestHandler
{
    public Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return input.ResponseBuilder
            .Speak("Sorry, I didn't understand that.")
            .GetResponse(cancellationToken);
    }
}

// You can override it manually if needed
services.AddTransient<IDefaultRequestHandler, CustomDefaultHandler>();
```

## Diagnostics

The source generator provides compile-time diagnostics:

### AVXC001: No Handlers Found
```
warning AVXC001: No request handlers found in assembly
```
**Cause**: No classes implementing `IRequestHandler<T>` or `IDefaultRequestHandler` were found.
**Solution**: Ensure your handlers implement the correct interfaces and are not marked with `[AlexaHandler(Exclude = true)]`.

### AVXC002: Multiple Default Handlers
```
error AVXC002: Multiple classes implement IDefaultRequestHandler
```
**Cause**: More than one class implements `IDefaultRequestHandler`.
**Solution**: Only one default handler is allowed. Remove or exclude extra implementations.

### AVXC003: Multiple Persistence Adapters
```
error AVXC003: Multiple classes implement IPersistenceAdapter
```
**Cause**: More than one class implements `IPersistenceAdapter`.
**Solution**: Only one persistence adapter is allowed per skill. Remove or exclude extra implementations.

## Viewing Generated Code

The generated interceptor code is written to your project's `obj/` folder:

```
obj/Debug/net10.0/generated/
  AlexaVoxCraft.MediatR.Generators/
    AlexaVoxCraft.MediatR.Generators.Generators.AlexaVoxCraftDiGenerator/
      __AlexaVoxCraft_Interceptors.g.cs
```

You can inspect this file to see exactly what registrations are being generated.

## Performance Impact

**Startup Time Comparison** (Lambda cold start):

| Method | Cold Start Time | Description |
|--------|----------------|-------------|
| Runtime Reflection | ~250ms | Scans assemblies, creates type instances |
| Source Generation | ~50ms | Direct method calls, no reflection |

**Benefits:**
- **80% faster** cold start initialization
- **No runtime overhead** for handler discovery
- **Smaller memory footprint** - no reflection metadata caching
- **Predictable performance** - same every time

## Best Practices

### 1. Use Source Generation (Default)

```csharp
// ✅ Good - source generation enabled (default)
services.AddSkillMediator(context.Configuration);

// ❌ Avoid - assembly scanning (only if source generation disabled)
services.AddSkillMediator(context.Configuration, cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());
```

### 2. Use Attributes for Control

```csharp
// Good - explicit control
[AlexaHandler(Lifetime = ServiceLifetime.Scoped, Order = 5)]
public class GameStateHandler : IRequestHandler<IntentRequest> { }

// Also good - uses sensible defaults
public class SimpleHandler : IRequestHandler<LaunchRequest> { }
```

### 3. Order Pipeline Components

```csharp
[AlexaHandler(Order = 1)]  // Authentication first
public class AuthInterceptor : IRequestInterceptor { }

[AlexaHandler(Order = 5)]  // Then logging
public class LogInterceptor : IRequestInterceptor { }

[AlexaHandler(Order = 10)] // Finally localization
public class LocalizationInterceptor : IRequestInterceptor { }
```

### 4. Keep Handlers Simple

```csharp
// Good - focused responsibility
public class LaunchHandler : IRequestHandler<LaunchRequest>
{
    private readonly IGameService _gameService;

    public LaunchHandler(IGameService gameService)
    {
        _gameService = gameService;
    }
}

// Avoid - too many dependencies, consider splitting
public class OverloadedHandler : IRequestHandler<IntentRequest>
{
    // 10+ constructor parameters - red flag
}
```

### 5. Document Handler Order

```csharp
/// <summary>
/// Handles user answers to trivia questions.
/// Executes after authentication (Order = 1) and state loading (Order = 5).
/// </summary>
[AlexaHandler(Order = 10)]
public class AnswerHandler : IRequestHandler<IntentRequest> { }
```

## Troubleshooting

### Interceptor Not Working

**Symptoms**: Runtime registration still being used, no compile-time interception.

**Solutions**:
1. Verify .NET SDK version: `dotnet --version` (must be 8.0.400+)
2. Check `<EnableMediatRGeneratorInterceptor>` is not set to `false`
3. Ensure you're calling the extension method, not a custom method
4. Clean and rebuild: `dotnet clean && dotnet build`

### Handlers Not Being Registered

**Symptoms**: Handler exists but not getting called.

**Solutions**:
1. Check handler implements correct interface (`IRequestHandler<T>`)
2. Verify handler is not marked `[AlexaHandler(Exclude = true)]`
3. Check `CanHandle()` logic returns `true` for expected requests
4. Review generated code in `obj/` folder to confirm registration

### Build Errors with Interceptors

**Symptoms**: CS errors about interceptors, method signatures.

**Solutions**:
1. Update to .NET SDK 8.0.400+ (interceptors require C# 12)
2. Ensure `<LangVersion>12</LangVersion>` or higher in csproj
3. Check Visual Studio is version 17.11 or higher
4. Try deleting `bin/` and `obj/` folders, then rebuild

### Multiple Default Handler Error

**Symptoms**: AVXC002 diagnostic at compile time.

**Solution**:
```csharp
// Option 1: Exclude one
[AlexaHandler(Exclude = true)]
public class OldDefaultHandler : IDefaultRequestHandler { }

// Option 2: Remove IDefaultRequestHandler from one
public class RegularHandler : IRequestHandler<IntentRequest> { }
```

## Migration from Runtime Registration

If upgrading from a version that used runtime registration:

**Before (v4.x with runtime registration):**
```csharp
services.AddSkillMediator(context.Configuration, cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.RegisterInterceptors(typeof(MyInterceptor).Assembly);
});
```

**After (v5.x with source generation - recommended):**
```csharp
// Simplified - everything auto-registered at compile time!
services.AddSkillMediator(context.Configuration);
```

**No code changes needed** - existing handlers work with source generation automatically. Just remove the assembly scanning configuration and enjoy faster startup times!

## Examples

See the [complete trivia skill implementation](../examples/index.md) for real-world usage of source-generated handlers with:
- Multiple request types
- Pipeline behaviors with ordering
- Exception handlers
- Request/response interceptors
- Custom lifetimes and configurations