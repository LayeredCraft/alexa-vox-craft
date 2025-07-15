# 🔣 AlexaVoxCraft

**AlexaVoxCraft** is a modular C# .NET library for building Amazon Alexa skills using modern .NET practices. It provides comprehensive support for Alexa skill development with CQRS patterns, visual interfaces, and AWS Lambda hosting.

## Key Features

- **🎯 MediatR Integration**: CQRS-style request handling with pipeline behaviors and auto-discovery
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
| **AlexaVoxCraft.MediatR.Lambda** | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda) | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda/) |
| **AlexaVoxCraft.MediatR**        | [![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR)               | [![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR/)               |

[![Build Status](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)

## 🚀 Quick Start

### Install Core Packages

```bash
# Core MediatR integration and Lambda hosting
dotnet add package AlexaVoxCraft.MediatR.Lambda

# APL visual interface support (optional)
dotnet add package AlexaVoxCraft.Model.Apl

# CloudWatch-compatible JSON logging (optional)
dotnet add package LayeredCraft.Logging.CompactJsonFormatter
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
                services.AddSkillMediator(context.Configuration, cfg => 
                    cfg.RegisterServicesFromAssemblyContaining<MySkillFunction>());
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
- **[APL Integration](https://layeredcraft.github.io/alexa-vox-craft/components/apl-integration/)** - Rich visual interface development
- **[Lambda Hosting](https://layeredcraft.github.io/alexa-vox-craft/components/lambda-hosting/)** - AWS Lambda deployment and optimization
- **[Session Management](https://layeredcraft.github.io/alexa-vox-craft/components/session-management/)** - State persistence and user data
- **[Pipeline Behaviors](https://layeredcraft.github.io/alexa-vox-craft/components/pipeline-behaviors/)** - Cross-cutting concerns and interceptors

### Examples

- **[Complete Examples](https://layeredcraft.github.io/alexa-vox-craft/examples/)** - Production-ready trivia skill implementation

## 📁 Project Structure

```
AlexaVoxCraft/
├── 📂 src/                              # Core library packages
│   ├── 📦 AlexaVoxCraft.Model/          # Base Alexa skill models & serialization
│   ├── 📦 AlexaVoxCraft.Model.Apl/      # APL (Alexa Presentation Language) support
│   ├── 📦 AlexaVoxCraft.MediatR/        # MediatR integration & request handling
│   ├── 📦 AlexaVoxCraft.MediatR.Lambda/ # AWS Lambda hosting & runtime
│
├── 📂 samples/                          # Working example projects
│   ├── 📱 Sample.Skill.Function/        # Basic Alexa skill demonstration
│   └── 📱 Sample.Apl.Function/          # APL skill with visual interfaces
│
├── 📂 test/                             # Comprehensive test coverage
│   ├── 🧪 AlexaVoxCraft.Model.Tests/    # Core model & serialization tests
│   ├── 🧪 AlexaVoxCraft.Model.Apl.Tests/ # APL functionality tests
│   ├── 🧪 AlexaVoxCraft.MediatR.Tests/  # MediatR integration tests
│   ├── 🧪 AlexaVoxCraft.MediatR.Lambda.Tests/ # Lambda hosting tests
│
├── 📂 AlexaVoxCraft.TestKit/            # Testing utilities & AutoFixture support
├── 📂 docs/                             # Documentation source
└── 📂 samples/                          # Example implementations
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