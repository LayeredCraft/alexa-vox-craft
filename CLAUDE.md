# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AlexaVoxCraft is a modular C# .NET library for building Amazon Alexa skills using modern .NET practices. It provides:

- **Core Models**: Complete Alexa skill request/response models with System.Text.Json serialization
- **APL Support**: Full Alexa Presentation Language implementation for visual interfaces
- **MediatR Integration**: CQRS-style request handling with pipeline behaviors
- **Lambda Hosting**: AWS Lambda runtime integration with custom serialization
- **Structured Logging**: Serilog integration with CloudWatch-compatible JSON formatting

## Architecture

### Core Components

- **AlexaVoxCraft.Model**: Base Alexa skill models (requests, responses, directives)
- **AlexaVoxCraft.Model.Apl**: Comprehensive APL components and document support
- **AlexaVoxCraft.MediatR**: Core MediatR integration for request handling patterns
- **AlexaVoxCraft.MediatR.Lambda**: AWS Lambda hosting with middleware
- **AlexaVoxCraft.Logging**: Alexa-specific logging components with AWS CloudWatch compatibility

### Request Handling Pattern

Skills use the MediatR pattern where:
1. Requests implement `IRequestHandler<T>` 
2. Handlers optionally implement `ICanHandle` for routing logic
3. Pipeline behaviors handle cross-cutting concerns (logging, exceptions)
4. Lambda functions derive from `AlexaSkillFunction<TRequest, TResponse>`

### Key Entry Point Pattern

```csharp
// Program.cs
return await LambdaHostExtensions.RunAlexaSkill<Function, SkillRequest, SkillResponse>();

// Function class
public class Function : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, SkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration, cfg => 
                    cfg.RegisterServicesFromAssemblyContaining<Program>());
            });
    }
}
```

## Development Commands

### Build and Test
```bash
# Build the entire solution
dotnet build AlexaVoxCraft.sln

# Run all tests
dotnet test

# Run tests for specific project
dotnet test test/AlexaVoxCraft.Model.Tests/

# Clean build artifacts
dotnet clean

# Restore dependencies
dotnet restore

# Pack NuGet packages
dotnet pack
```

### Lambda Development
```bash
# Build Lambda-ready packages (from sample directories)
dotnet publish -c Release --runtime linux-x64 --self-contained false

# Test Lambda functions locally (requires AWS Lambda Mock Test Tool)
dotnet run
```

## Testing Architecture

- **xUnit**: Primary testing framework
- **JSON Examples**: Extensive test data files in `/test/` directories
- **Serialization Tests**: Validation of System.Text.Json converters
- **APL Component Tests**: Comprehensive APL document and component validation

## Logging Configuration

**AlexaVoxCraft.Logging** now builds on **LayeredCraft.StructuredLogging** for general logging functionality and provides Alexa-specific components:
- `AlexaCompactJsonFormatter`: AWS CloudWatch-compatible JSON formatter 
- Renames reserved fields: `@t` → `_t`, `@l` → `_l`, `@m` → `_m`
- For structured logging extensions, use `LayeredCraft.StructuredLogging.Extensions`

Enable request/response logging in development:
```json
"AlexaVoxCraft.MediatR.Lambda.Serialization": "Debug"
```

## Key Patterns

### Request Handlers
```csharp
public class LaunchRequestHandler : IRequestHandler<LaunchRequest>
{
    public bool CanHandle(IHandlerInput handlerInput) => 
        handlerInput.RequestEnvelope.Request is LaunchRequest;

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return await input.ResponseBuilder
            .Speak("Hello world!")
            .WithShouldEndSession(true)
            .GetResponse(cancellationToken);
    }
}
```

### Exception Handling
Exception handlers implement `IExceptionHandler` and are auto-registered:
```csharp
public class MyExceptionHandler : IExceptionHandler
{
    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        var response = handlerInput.ResponseBuilder.Speak("Something went wrong.");
        return response.GetResponse(cancellationToken);
    }
}
```

## Project Structure

- `/src/`: Library source code (5 NuGet packages)
- `/samples/`: Working example skills (basic and APL)
- `/test/`: Unit tests with extensive JSON example files
- **Multi-targeting**: Supports .NET 8.0 and .NET 9.0
- **Lambda Optimized**: ReadyToRun publishing, ICU bundling, self-contained deployment options

## Key Features

- **Polymorphic JSON**: Custom converters for complex Alexa object hierarchies
- **APL Extensions**: Support for BackStack, DataStore, EntitySensing, SmartMotion
- **Session Management**: Context and state management for multi-turn conversations
- **Connection Tasks**: Support for complex multi-step interactions
- **AWS Integration**: Lambda runtime support with custom serialization