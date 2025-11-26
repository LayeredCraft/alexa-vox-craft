---
layout: default
title: AlexaVoxCraft
---

[![Build Status](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)
[![NuGet](https://img.shields.io/nuget/v/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda/)
[![Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda/)

AlexaVoxCraft is a modular C# .NET library for building Amazon Alexa skills using modern .NET practices. It provides comprehensive support for Alexa skill development with CQRS patterns, visual interfaces, and AWS Lambda hosting.

## Key Features

- **🎯 MediatR Integration**: CQRS-style request handling with compile-time source-generated DI registration
- **🎨 APL Support**: Complete Alexa Presentation Language implementation for rich visual interfaces
- **⚡ Lambda Hosting**: Optimized AWS Lambda runtime with custom serialization and ReadyToRun publishing
- **📊 Session Management**: Robust session attribute handling and game state persistence
- **🔧 Pipeline Behaviors**: Request/response interceptors for cross-cutting concerns like logging and validation
- **🧪 Testing Support**: Comprehensive testing utilities with AutoFixture integration and property-based testing

## Installation

```bash
# Core MediatR integration and Lambda hosting
dotnet add package AlexaVoxCraft.MediatR.Lambda

# APL visual interface support
dotnet add package AlexaVoxCraft.Model.Apl

# Structured logging for Alexa skills
dotnet add package AlexaVoxCraft.Logging

# OpenTelemetry observability (optional)
dotnet add package AlexaVoxCraft.Observability
```

### Requirements

⚠️ **SDK Version Required**: To use source-generated dependency injection with interceptors, you must use at least version **8.0.400** of the .NET SDK. This ships with **Visual Studio 2022 version 17.11** or higher.

```bash
# Check your SDK version
dotnet --version
# Should show 8.0.400 or higher
```

## Quick Start - Building a Trivia Skill

All code examples in this documentation demonstrate building a **trivia game skill**. Here's a complete implementation:

```csharp
// Program.cs
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Response;

APLSupport.Add();

return await LambdaHostExtensions.RunAlexaSkill<TriviaSkillFunction, APLSkillRequest, SkillResponse>();

// Function class
public class TriviaSkillFunction : AlexaSkillFunction<APLSkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                // Handlers are automatically discovered and registered at compile time
                services.AddSkillMediator(context.Configuration);

                // Add your business services
                services.AddScoped<IGameService, GameService>();
                services.AddScoped<IQuestionRepository, QuestionRepository>();
            });
    }
}

// Request Handler
public class LaunchRequestHandler : IRequestHandler<LaunchRequest>
{
    public bool CanHandle(IHandlerInput handlerInput) => 
        handlerInput.RequestEnvelope.Request is LaunchRequest;

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return await input.ResponseBuilder
            .Speak("Welcome to Trivia Challenge! Ready to test your knowledge?")
            .Reprompt("Say yes to start playing, or help for instructions.")
            .WithSimpleCard("Trivia Challenge", "Welcome to Trivia Challenge!")
            .GetResponse(cancellationToken);
    }
}
```

## Available Components

> 📝 **Note**: All component documentation uses trivia skill examples to demonstrate features and patterns.

### [Request Handling](components/request-handling.md)

Modern request handling patterns with trivia game examples:

- MediatR integration for CQRS
- Compile-time source-generated handler registration
- Type-safe request/response processing
- Exception handling pipeline
- Request validation and routing

### [Source Generation](components/source-generation.md)

Compile-time dependency injection with C# interceptors:

- Zero runtime reflection - faster Lambda cold starts
- Automatic handler discovery at compile time
- Customizable registration with attributes
- Execution ordering for pipeline behaviors
- Type-safe validation during build

### [APL Integration](components/apl-integration.md)

Rich visual interface support for trivia questions with:

- Fluent document builder API
- Multi-slide presentations
- Interactive components
- Voice/touch synchronization
- Custom layout management

### [Lambda Hosting](components/lambda-hosting.md)

Optimized AWS Lambda runtime for trivia skills with:

- Custom serialization for Alexa models
- ReadyToRun publishing for faster cold starts
- Self-contained deployment options
- Environment-specific configuration
- Comprehensive logging integration

### [Session Management](components/session-management.md)

Robust game state management with:

- Session attribute persistence
- Game state tracking
- User data management
- Multi-turn conversation support
- DynamoDB integration patterns

### [Pipeline Behaviors](components/pipeline-behaviors.md)

Cross-cutting concerns for skill management with:

- Request/response interceptors
- Logging and telemetry
- User authentication and authorization
- Error handling and recovery
- Performance monitoring

### [OpenTelemetry Observability](components/observability.md)

Comprehensive observability for production skills with:

- Standards-compliant OpenTelemetry instrumentation
- Request processing spans with semantic attributes
- Performance metrics and cold start tracking
- Privacy-safe session correlation
- Zero-configuration, opt-in telemetry

## Documentation

- **[Examples](examples/index.md)** - Complete trivia skill implementation with real-world patterns

## Real-World Example

Our documentation is built around a complete, production-ready trivia skill that demonstrates:

- **Game Logic**: Question management, scoring, and leaderboards
- **Visual Interface**: APL documents with interactive slides
- **Data Persistence**: DynamoDB integration for user and game data
- **Deployment**: AWS Lambda with CDK infrastructure

See the [Examples](examples/index.md) section for the complete implementation.

## Runtime Requirements

- **.NET 8.0** or **.NET 9.0**
- **.NET SDK 8.0.400+** for source generation (VS 2022 17.11+)
- **AWS Lambda Runtime** (provided.al2023)
- **Amazon Alexa Developer Account**
- **AWS CLI** configured with appropriate permissions

## Architecture Patterns

AlexaVoxCraft follows these architectural patterns:

1. **CQRS with MediatR**: Separate command and query responsibilities
2. **Pipeline Pattern**: Composable request/response processing
3. **Builder Pattern**: Fluent APIs for APL documents and responses
4. **Repository Pattern**: Data access abstraction for session and user data
5. **Dependency Injection**: Service registration and lifetime management

## Contributing

See the main [README](https://github.com/LayeredCraft/alexa-vox-craft#contributing) for contribution guidelines.

## License

This project is licensed under the [MIT License](https://github.com/LayeredCraft/alexa-vox-craft/blob/main/LICENSE).