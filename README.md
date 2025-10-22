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

| Package                          | NuGet                                                                                                                                       | Downloads                                                                                                                                            |
|----------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------|
| **AlexaVoxCraft.Model**          | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Model.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model)                   | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Model.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model/)                   |
| **AlexaVoxCraft.Model.Apl**      | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Model.Apl.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model.Apl)           | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Model.Apl.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model.Apl/)           |
| **AlexaVoxCraft.MediatR**        | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR)               | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR/)               |
| **AlexaVoxCraft.MediatR.Lambda** | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda) | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda/) |
| **AlexaVoxCraft.Observability**  | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.Observability.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Observability)   | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Observability.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Observability/)   |

[![Build Status](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)

## 🚀 Quick Start

### Install Core Packages

```bash
# Core MediatR integration and Lambda hosting
dotnet add package AlexaVoxCraft.MediatR.Lambda

# APL visual interface support (optional)
dotnet add package AlexaVoxCraft.Model.Apl

# OpenTelemetry observability (optional)
dotnet add package AlexaVoxCraft.Observability

# CloudWatch-compatible JSON logging (optional)
dotnet add package LayeredCraft.Logging.CompactJsonFormatter
```

### Requirements

⚠️ **SDK Version Required**: To use source-generated dependency injection with interceptors, you must use at least version **8.0.400** of the .NET SDK. This ships with **Visual Studio 2022 version 17.11** or higher.

```bash
# Check your SDK version
dotnet --version
# Should show 8.0.400 or higher
```

### Create a Basic Skill

```csharp
// Program.cs
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Response;

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
                // Handlers are automatically discovered and registered at compile time
                services.AddSkillMediator(context.Configuration);
            });
    }
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

📚 **[Complete Documentation](https://layeredcraft.github.io/alexa-vox-craft/)** - Comprehensive guides and examples

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
│   ├── 📦 AlexaVoxCraft.MediatR.Lambda/       # AWS Lambda hosting & runtime
│   └── 📦 AlexaVoxCraft.Observability/        # OpenTelemetry instrumentation & telemetry
│
├── 📂 samples/                                # Working example projects
│   ├── 📱 Sample.Skill.Function/              # Basic Alexa skill demonstration
│   ├── 📱 Sample.Generated.Function/          # Source-generated DI demonstration
│   └── 📱 Sample.Apl.Function/                # APL skill with visual interfaces
│
├── 📂 test/                                   # Comprehensive test coverage
│   ├── 🧪 AlexaVoxCraft.Model.Tests/          # Core model & serialization tests
│   ├── 🧪 AlexaVoxCraft.Model.Apl.Tests/      # APL functionality tests
│   ├── 🧪 AlexaVoxCraft.MediatR.Tests/        # MediatR integration tests
│   └── 🧪 AlexaVoxCraft.MediatR.Lambda.Tests/ # Lambda hosting tests
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
| **AlexaVoxCraft.MediatR.Lambda** | Lambda hosting | AWS Lambda functions, context management, custom serialization, hosting extensions |
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
