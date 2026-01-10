# 🔣 AlexaVoxCraft

**AlexaVoxCraft** is a modular C# .NET library for building Amazon Alexa skills using modern .NET practices. It provides comprehensive support for Alexa skill development with CQRS patterns, visual interfaces, and AWS Lambda hosting.

## Key Features

- **🎯 MediatR Integration**: CQRS-style request handling with compile-time source-generated DI registration
- **🎨 APL Support**: Complete Alexa Presentation Language implementation for rich visual interfaces
- **⚡ Lambda Hosting**: Optimized AWS Lambda runtime with custom serialization and ReadyToRun publishing
- **📊 Session Management**: Robust session attribute handling and game state persistence
- **🔧 Pipeline Behaviors**: Request/response interceptors for cross-cutting concerns like logging and validation
- **🧪 Testing Support**: Comprehensive testing utilities with AutoFixture integration and property-based testing

## 📦 Packages

| Package                             | NuGet                                                                                                                                         | Downloads                                                                                                                                                  |
|-------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **AlexaVoxCraft.Model**             | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Model.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model)                        | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Model.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model/)                               |
| **AlexaVoxCraft.Model.Apl**         | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Model.Apl.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model.Apl)                | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Model.Apl.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model.Apl/)                       |
| **AlexaVoxCraft.MediatR**           | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR)                    | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR/)                           |
| **AlexaVoxCraft.Lambda**            | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Lambda)                      | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Lambda/)                             |
| **AlexaVoxCraft.MinimalLambda**     | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MinimalLambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MinimalLambda)        | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MinimalLambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MinimalLambda/)               |
| **AlexaVoxCraft.MediatR.Lambda**    | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda)     | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda/)             |
| **AlexaVoxCraft.Observability**     | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Observability.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Observability)       | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Observability.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Observability/)               |
| **AlexaVoxCraft.Smapi**             | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Smapi.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Smapi)                        | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Smapi.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Smapi/)                               |

[![Build Status](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)

## 🚀 Quick Start

### Install Core Packages

```bash
# Minimal Lambda hosting and MediatR integration
dotnet add package AlexaVoxCraft.MinimalLambda
dotnet add package AlexaVoxCraft.MediatR

# APL visual interface support (optional)
dotnet add package AlexaVoxCraft.Model.Apl

# OpenTelemetry observability (optional)
dotnet add package AlexaVoxCraft.Observability

# CloudWatch-compatible JSON logging (optional)
dotnet add package LayeredCraft.Logging.CompactJsonFormatter
```

### Requirements

Supported target frameworks: **.NET 8.0**, **.NET 9.0**, **.NET 10.0**.

⚠️ **SDK Version Required**: To use source-generated dependency injection with interceptors, you must use at least version **8.0.400** of the .NET SDK. This ships with **Visual Studio 2022 version 17.11** or higher.

```bash
# Check your SDK version
dotnet --version
# Should show 8.0.400 or higher
```

### Create a Basic Skill

```csharp
// Program.cs (MinimalLambda host)
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MinimalLambda;
using AlexaVoxCraft.MinimalLambda.Extensions;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using MinimalLambda.Builder;

var builder = LambdaApplication.CreateBuilder();

// Handlers are automatically discovered and registered at compile time
builder.Services.AddSkillMediator(builder.Configuration);

// Register AlexaVoxCraft hosting services and handler
builder.Services.AddAlexaSkillHost<LambdaHandler, SkillRequest, SkillResponse>();

await using var app = builder.Build();
app.MapHandler(AlexaHandler.Invoke<SkillRequest, SkillResponse>);
return await app.RunAsync();

// Lambda handler bridges MediatR to the MinimalLambda host
public class LambdaHandler : ILambdaHandler<SkillRequest, SkillResponse>
{
    private readonly ISkillMediator _mediator;

    public LambdaHandler(ISkillMediator mediator) => _mediator = mediator;

    public Task<SkillResponse> HandleAsync(SkillRequest request, ILambdaContext context, CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);
}

// Handler.cs
public class LaunchRequestHandler : IRequestHandler<LaunchRequest>
{
    public bool CanHandle(IHandlerInput handlerInput) =>
        handlerInput.RequestEnvelope.Request is LaunchRequest;

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return await input.ResponseBuilder
            .Speak("Welcome to my skill!")
            .Reprompt("What would you like to do?")
            .GetResponse(cancellationToken);
    }
}
```

## 📖 Documentation

📚 **[Complete Documentation](https://alexavoxcraft.layeredcraft.dev/)** - Comprehensive guides and examples

### Core Components

- **[Request Handling](https://layeredcraft.github.io/alexa-vox-craft/components/request-handling/)** - MediatR integration and handler patterns
- **[Source Generation](https://layeredcraft.github.io/alexa-vox-craft/components/source-generation/)** - Compile-time DI registration with C# interceptors
- **[APL Integration](https://layeredcraft.github.io/alexa-vox-craft/components/apl-integration/)** - Rich visual interface development
- **[Lambda Hosting](https://layeredcraft.github.io/alexa-vox-craft/components/lambda-hosting/)** - AWS Lambda deployment and optimization
- **[Session Management](https://layeredcraft.github.io/alexa-vox-craft/components/session-management/)** - State persistence and user data
- **[Pipeline Behaviors](https://layeredcraft.github.io/alexa-vox-craft/components/pipeline-behaviors/)** - Cross-cutting concerns and interceptors

### Examples

- **[Complete Examples](https://layeredcraft.github.io/alexa-vox-craft/examples/)** - Production-ready trivia skill implementation

## 📁 Project Structure

```
AlexaVoxCraft/
├── 📂 src/                                    # Core library packages
│   ├── 📦 AlexaVoxCraft.Model/                # Base Alexa skill models & serialization
│   ├── 📦 AlexaVoxCraft.Model.Apl/            # APL (Alexa Presentation Language) support
│   ├── 📦 AlexaVoxCraft.MediatR/              # MediatR integration & request handling
│   ├── 📦 AlexaVoxCraft.MediatR.Generators/   # Source generator for compile-time DI
│   ├── 📦 AlexaVoxCraft.Lambda/               # Core Lambda abstractions & serialization
│   ├── 📦 AlexaVoxCraft.MinimalLambda/        # MinimalLambda-based hosting for Alexa skills
│   ├── 📦 AlexaVoxCraft.MediatR.Lambda/       # Legacy Lambda hosting (AlexaSkillFunction)
│   ├── 📦 AlexaVoxCraft.Observability/        # OpenTelemetry instrumentation & telemetry
│   └── 📦 AlexaVoxCraft.Smapi/                # Skill Management API (SMAPI) client
│
├── 📂 samples/                                # Working example projects
│   ├── 📱 Sample.Skill.Function/              # Basic skill (legacy hosting)
│   ├── 📱 Sample.Host.Function/               # Modern minimal API hosting
│   ├── 📱 Sample.Generated.Function/          # Source-generated DI demonstration
│   └── 📱 Sample.Apl.Function/                # APL skill with visual interfaces
│
├── 📂 test/                                   # Comprehensive test coverage
│   ├── 🧪 AlexaVoxCraft.Model.Tests/          # Core model & serialization tests
│   ├── 🧪 AlexaVoxCraft.Model.Apl.Tests/      # APL functionality tests
│   ├── 🧪 AlexaVoxCraft.MediatR.Tests/        # MediatR integration tests
│   ├── 🧪 AlexaVoxCraft.MediatR.Lambda.Tests/ # Lambda hosting tests
│   └── 🧪 AlexaVoxCraft.Smapi.Tests/          # SMAPI client tests
│
├── 📂 AlexaVoxCraft.TestKit/                  # Testing utilities & AutoFixture support
└── 📂 docs/                                   # Documentation source
```

## 🛠 Core Concepts

### Request Handling Pattern

Skills use the MediatR pattern where:
1. Requests implement `IRequestHandler<T>` 
2. Handlers optionally implement `ICanHandle` for routing logic
3. Pipeline behaviors handle cross-cutting concerns (logging, exceptions)
4. Lambda functions derive from `AlexaSkillFunction<TRequest, TResponse>`

### Package Breakdown

| Package | Purpose | Key Features |
|---------|---------|--------------|
| **AlexaVoxCraft.Model** | Core Alexa models | Request/response types, SSML, cards, directives, System.Text.Json serialization |
| **AlexaVoxCraft.Model.Apl** | APL support | 40+ components, commands, audio, vector graphics, extensions (DataStore, SmartMotion) |
| **AlexaVoxCraft.MediatR** | Request handling | Handler routing, pipeline behaviors, attributes management, DI integration |
| **AlexaVoxCraft.MinimalLambda** | MinimalLambda hosting | Minimal API-style hosting for Alexa skills, custom serialization, handler mapping |
| **AlexaVoxCraft.MediatR.Lambda** | Legacy Lambda hosting | AWS Lambda functions, context management, custom serialization, hosting extensions |
| **AlexaVoxCraft.Observability** | OpenTelemetry integration | Opt-in telemetry, metrics, spans, semantic attributes, ADOT/CloudWatch support |

## 🧪 Testing

AlexaVoxCraft includes comprehensive testing support:

- **xUnit v3** with Microsoft.Testing.Platform
- **AutoFixture** integration for property-based testing
- **AwesomeAssertions** for fluent assertions
- **TestKit** with specimen builders and test utilities

## ⚠️ Error Handling

Implement the `IExceptionHandler` interface for centralized error handling:

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        return Task.FromResult(true); // Handle all exceptions
    }

    public Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        return handlerInput.ResponseBuilder
            .Speak("Sorry, something went wrong. Please try again.")
            .GetResponse(cancellationToken);
    }
}
```

## 📋 Version 4.0.0+ Breaking Changes

### Cancellation Token Support

Version 4.0.0 introduces cancellation token support for Lambda functions. This is a **breaking change** that requires updates to your existing lambda handlers.

#### Required Changes

**1. Update Lambda Handlers**

Lambda handlers must now accept and pass the cancellation token:

```csharp
// ❌ Before (v3.x)
public class LambdaHandler : ILambdaHandler<SkillRequest, SkillResponse>
{
    public async Task<SkillResponse> HandleAsync(SkillRequest input, ILambdaContext context)
    {
        return await _skillMediator.Send(input);
    }
}

// ✅ After (v4.0+)
public class LambdaHandler : ILambdaHandler<SkillRequest, SkillResponse>
{
    public async Task<SkillResponse> HandleAsync(SkillRequest input, ILambdaContext context, CancellationToken cancellationToken)
    {
        return await _skillMediator.Send(input, cancellationToken);
    }
}
```

**2. Update AlexaSkillFunction Override**

If you override `FunctionHandlerAsync` in your skill function class, you must update the signature:

```csharp
// ❌ Before (v3.x)
public class MySkillFunction : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    public override async Task<SkillResponse> FunctionHandlerAsync(SkillRequest input, ILambdaContext context)
    {
        // Custom logic here
        return await base.FunctionHandlerAsync(input, context);
    }
}

// ✅ After (v4.0+)
public class MySkillFunction : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    public override async Task<SkillResponse> FunctionHandlerAsync(SkillRequest input, ILambdaContext context, CancellationToken cancellationToken)
    {
        // Custom logic here
        return await base.FunctionHandlerAsync(input, context, cancellationToken);
    }
}
```

#### Configuration Options

You can now configure the cancellation timeout buffer in your `appsettings.json`:

```json
{
  "SkillConfiguration": {
    "SkillId": "amzn1.ask.skill.your-skill-id",
    "CancellationTimeoutBufferMilliseconds": 500
  }
}
```

The timeout buffer (default: 250ms) is subtracted from Lambda's remaining execution time to allow graceful shutdown and telemetry flushing.

## 📋 Version 5.1.0+ Changes

### Overview

Version 5.1.0 (beta) introduces the `AlexaVoxCraft.MinimalLambda` package, replacing the previous `AlexaVoxCraft.Lambda.Host` host with a MinimalLambda-based experience. The legacy hosting approach remains fully supported for backward compatibility.

### Package Restructuring

Version 5.1.0 refines the hosting lineup:

| Package | Purpose | Status |
|---------|---------|--------|
| **AlexaVoxCraft.Lambda** | Core Lambda abstractions and serialization | Active |
| **AlexaVoxCraft.MinimalLambda** | MinimalLambda-based hosting | New (recommended for new projects) |
| **AlexaVoxCraft.MediatR.Lambda** | Legacy Lambda hosting with AlexaSkillFunction | Existing (still fully supported) |

### Namespace Changes

Core Lambda abstractions have been moved to the new `AlexaVoxCraft.Lambda` package:

**Classes Moved:**
- `ILambdaHandler<TRequest, TResponse>`
- `HandlerDelegate<TRequest, TResponse>`
- `AlexaLambdaSerializer`
- `SystemTextDestructuringPolicy`

### Removed Obsolete Classes

The following obsolete class has been removed:

**PollyVoices** - This class was marked as obsolete in previous versions and has been removed. Use the `AlexaSupportedVoices` class instead for Amazon Polly voice name constants.

**Before (v4.x):**
```csharp
using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.Lambda.Serialization;

public class LambdaHandler : ILambdaHandler<SkillRequest, SkillResponse>
{
    // Implementation
}
```

**After (v5.0+):**
```csharp
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.Lambda.Serialization;

public class LambdaHandler : ILambdaHandler<SkillRequest, SkillResponse>
{
    // Implementation
}
```

### Two Hosting Approaches

Version 5.1.0 provides two ways to host Alexa skills in AWS Lambda:

#### 🌟 Modern Approach (Recommended for New Projects)

Use `AlexaVoxCraft.MinimalLambda` for a familiar minimal API-style hosting experience:

```bash
dotnet add package AlexaVoxCraft.MinimalLambda
```

```csharp
// Program.cs
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MinimalLambda;
using AlexaVoxCraft.MinimalLambda.Extensions;
using AlexaVoxCraft.Model.Response;
using MinimalLambda.Builder;

var builder = LambdaApplication.CreateBuilder();

builder.Services.AddSkillMediator(builder.Configuration);
builder.Services.AddAlexaSkillHost<LambdaHandler, SkillRequest, SkillResponse>();

await using var app = builder.Build();
app.MapHandler(AlexaHandler.Invoke<SkillRequest, SkillResponse>);
await app.RunAsync();
```

**Benefits:**
- Familiar minimal API-style builder pattern (similar to ASP.NET Core)
- Lean MinimalLambda runtime keeps cold starts small
- More flexible service configuration
- Better separation of concerns

#### Legacy Approach (Existing Projects)

Continue using `AlexaVoxCraft.MediatR.Lambda` with the `AlexaSkillFunction` pattern:

```bash
dotnet add package AlexaVoxCraft.MediatR.Lambda
```

```csharp
// Program.cs
using AlexaVoxCraft.MediatR.Lambda;

return await LambdaHostExtensions.RunAlexaSkill<MySkillFunction, SkillRequest, SkillResponse>();

// Function.cs
public class MySkillFunction : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, SkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration);
            });
    }
}
```

This approach remains **fully supported** and requires no migration for existing projects.

### Migration Impact

**Who is affected:**
- Projects directly referencing `AlexaVoxCraft.MediatR.Lambda.Abstractions` namespace
- Projects directly referencing `AlexaVoxCraft.MediatR.Lambda.Serialization` namespace
- Projects wanting to adopt the modern hosting approach

**Who is NOT affected:**
- Projects using `AlexaSkillFunction<TRequest, TResponse>` without directly referencing moved classes
- Projects that don't import the moved namespaces

### Required Actions for Migration

#### 1. Update Namespace Imports

If you directly reference the moved classes, update your using statements:

```csharp
// Change this:
using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.Lambda.Serialization;

// To this:
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.Lambda.Serialization;
```

#### 2. (Optional) Migrate to Modern Hosting

To adopt the new minimal API-style hosting:

1. Replace package reference:
   ```bash
   dotnet remove package AlexaVoxCraft.MediatR.Lambda
   dotnet add package AlexaVoxCraft.MinimalLambda
   dotnet add package AlexaVoxCraft.MediatR
   ```

2. Remove `AwsLambda.Host` interceptor namespaces (not needed with MinimalLambda) and keep your generator interceptors.

3. Refactor Program.cs to use builder pattern (see example above)

4. Remove Function class inheriting from `AlexaSkillFunction`

For detailed migration guidance, see the [Lambda Hosting documentation](https://layeredcraft.github.io/alexa-vox-craft/components/lambda-hosting/).

## 📋 Version 6.0.0 Breaking Changes

### APL Collection Type System Overhaul

Version 6.0.0 introduces a major redesign of the APL collection type system with **breaking changes** that affect how you work with APL component arrays and collections.

#### What Changed

**1. Migration from APLValue<IList<T>> to APLValueCollection<T>**

All properties that previously used `APLValue<IList<T>>` or `APLValue<List<T>>` have been migrated to `APLValueCollection<T>`:

```csharp
// ❌ Before (v5.x)
public APLValue<IList<APLComponent>>? Items { get; set; }
public APLValue<List<int>>? Padding { get; set; }

// ✅ After (v6.0.0+)
public APLValueCollection<APLComponent>? Items { get; set; }
public APLValueCollection<int>? Padding { get; set; }
```

**2. APLValue<T> No Longer Supports Collection Types**

`APLValue<T>` now throws `InvalidOperationException` in its static constructor if `T` is any collection type (IEnumerable, ICollection, IList, List, etc.):

```csharp
// ❌ This will throw InvalidOperationException at runtime
var value = new APLValue<List<string>>();

// ✅ Use APLValueCollection<T> instead
var collection = new APLValueCollection<string>();
```

**3. APLValueCollection<T> Implements IList<T>**

`APLValueCollection<T>` now implements `IList<T>` and `IReadOnlyList<T>` directly, providing natural collection ergonomics:

```csharp
// ❌ Before (v5.x) - awkward .Items property access
collection.Items!.Add(new Text());
var count = collection.Items?.Count ?? 0;
var first = collection.Items![0];

// ✅ After (v6.0.0+) - direct collection operations
collection.Add(new Text());
var count = collection.Count;
var first = collection[0];
```

**4. Items Property Changed to Read-Only**

The `Items` property is now `IReadOnlyList<T>` (read-only) instead of `IList<T>?` (settable):

```csharp
// ❌ Before (v5.x) - Items was settable
collection.Items = new List<APLComponent> { new Text() };

// ✅ After (v6.0.0+) - Items is read-only, use constructor or Add()
var collection = new APLValueCollection<APLComponent>([new Text()]);
// OR
collection.Add(new Text());
```

**5. Materialize Pattern for Expression Handling**

Mutations automatically clear the `Expression` property when you modify the collection:

```csharp
// Expression-backed collection (data binding)
APLValueCollection<APLComponent> collection = "${data.items}";
collection.Expression; // "${data.items}"

// Mutation materializes the collection
collection.Add(new Text());
collection.Expression; // null - expression cleared

// Read operations preserve the expression
var count = collection.Count;  // Does NOT clear Expression
var contains = collection.Contains(item);  // Does NOT clear Expression
```

#### Migration Guide

**Step 1: Update Property Types**

If you have custom APL components or are working with the model directly:

```csharp
// Change this:
public APLValue<IList<APLComponent>>? Children { get; set; }
public APLValue<List<int>>? Padding { get; set; }

// To this:
public APLValueCollection<APLComponent>? Children { get; set; }
public APLValueCollection<int>? Padding { get; set; }
```

**Step 2: Remove .Items Property Access for Mutations**

```csharp
// ❌ Before (v5.x)
container.Items!.Add(new Text { Content = "Hello" });
container.Items!.Clear();
var component = container.Items![0];

// ✅ After (v6.0.0+)
container.Add(new Text { Content = "Hello" });
container.Clear();
var component = container[0];
```

**Step 3: Update Collection Initialization**

Collections can be initialized using collection expressions (C# 12), constructors, or implicit conversions:

```csharp
// Collection expression (C# 12+)
APLValueCollection<APLComponent> items = [
    new Text { Content = "Item 1" },
    new Text { Content = "Item 2" }
];

// Constructor with IEnumerable
var list = new List<APLComponent> { new Text() };
var collection = new APLValueCollection<APLComponent>(list);

// Implicit conversion
APLValueCollection<APLComponent> fromList = new List<APLComponent> { new Text() };
APLValueCollection<APLComponent> fromArray = new[] { new Text() };
APLValueCollection<APLComponent> fromExpression = "${data.items}";
```

**Step 4: Be Aware of Materialize Behavior**

If you're using expression-based binding and then mutating the collection:

```csharp
// This will clear the expression
var collection = new APLValueCollection<APLComponent> { Expression = "${data.items}" };
collection.Add(new Text());  // Expression is now null

// If you need to preserve expression mode, don't mutate the collection
// Use expression-only or items-only, not both
```

#### What Still Works

- ✅ Collection expressions: `[item1, item2, item3]`
- ✅ LINQ operations: `collection.OfType<Text>().ToList()`
- ✅ Foreach enumeration: `foreach (var item in collection)`
- ✅ Implicit conversions from `List<T>`, `T[]`, and `string`
- ✅ Expression-based data binding: `"${data.items}"`
- ✅ JSON serialization (unchanged behavior)
- ✅ The `Items` property for read-only access and inspection

#### Benefits of This Change

1. **Natural API**: Direct collection operations without `.Items` indirection
2. **Type Safety**: Cannot accidentally use `APLValue<List<T>>` anymore
3. **Better IntelliSense**: Collection methods appear directly on the type
4. **Clearer Intent**: Materialize pattern makes expression/items relationship explicit
5. **Standard .NET Patterns**: Implements `IList<T>` like other collection types

#### Impact Summary

**High Impact:**
- Custom APL component implementations
- Code directly manipulating APL collections
- Any code using `APLValue<IList<T>>` or `APLValue<List<T>>`

**Low Impact:**
- Most skill code using the document builder API
- Code that only reads from collections
- Expression-based data binding (still works)

## 🤝 Contributing

PRs are welcome! Please submit issues and ideas to help make this toolkit even better.

## 📜 Credits & Attribution

> 📦 **Credits:**
>
> - Core Alexa skill models (`AlexaVoxCraft.Model`) based on [timheuer/alexa-skills-dotnet](https://github.com/timheuer/alexa-skills-dotnet)
> - APL support (`AlexaVoxCraft.Model.Apl`) based on [stoiveyp/Alexa.NET.APL](https://github.com/stoiveyp/Alexa.NET.APL)

## 📜 License

This project is licensed under the [MIT License](LICENSE).

## Stargazers over time


[![Stargazers over time](https://starchart.cc/LayeredCraft/alexa-vox-craft.svg)](https://starchart.cc/LayeredCraft/alexa-vox-craft)
