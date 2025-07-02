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
# Install AWS Lambda Tools (one time setup)
dotnet new tool-manifest --force
dotnet tool install Amazon.Lambda.Tools
dotnet restore

# Create Lambda deployment package using aws-lambda-tools-defaults.json configuration
dotnet lambda package

# Test Lambda functions locally (requires AWS Lambda Mock Test Tool)
dotnet run
```

Use `aws-lambda-tools-defaults.json` in skill projects to configure Lambda packaging:
```json
{
  "Information": [
    "This file provides default values for the deployment wizard inside Visual Studio and the AWS Lambda commands added to the .NET Core CLI.",
    "To learn more about the Lambda commands with the .NET Core CLI execute the following command at the command line in the project root directory.",
    "dotnet lambda help",
    "All the command line options for the Lambda command can be specified in this file."
  ],
  "profile": "",
  "region": "",
  "configuration": "Release",
  "function-runtime": "provided.al2023",
  "function-memory-size": 512,
  "function-timeout": 30,
  "function-handler": "bootstrap",
  "msbuild-parameters": "--self-contained true"
}
```

## Testing Architecture

- **xUnit v3 with Microsoft.Testing.Platform**: Primary testing framework (use `dotnet run` not `dotnet test`)
- **AutoFixture**: Property-based testing with generated test data via TestKit
- **AwesomeAssertions**: Fluent assertion library (same API as FluentAssertions but different package)
- **TestKit Infrastructure**: Centralized testing utilities and specimen builders
- **JSON Examples**: Extensive test data files for deserialization validation
- **Serialization Tests**: Round-trip validation of System.Text.Json converters
- **APL Component Tests**: Comprehensive APL document and component validation

### TestKit Components
- **ModelAutoDataAttribute**: AutoFixture configuration for Model tests
- **CardSpecimenBuilder**: Generates realistic card objects with Alexa constraints
- **JsonTestExtensions**: Round-trip serialization validation utilities
- **TestLoggerCustomization**: Structured logging testing with proper freezing behavior

### Testing Notes
- Use `AwesomeAssertions` package, NOT `FluentAssertions` (same API, different package)
- Run tests with `dotnet run` due to Microsoft.Testing.Platform integration
- Use `[Theory]` with `[ModelAutoData]` for property-based testing with generated data

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

# Custom Instructions for Claude

## Code Style and Conventions
- NEVER add code comments unless explicitly requested
- Follow existing C# coding conventions and patterns in the codebase
- Use existing libraries and utilities - check imports and package.json/csproj before adding new dependencies
- When creating new components, examine existing similar components for patterns
- Prefer sealed classes when possible for better performance and design clarity

## Testing Guidelines
- Always use AutoFixture with TestKit for new Model tests
- Use `AwesomeAssertions` for assertions (same API as FluentAssertions)
- Create property-based tests with `[Theory]` and `[ModelAutoData]`
- Validate both object constraints AND round-trip serialization
- Use `dotnet run` instead of `dotnet test` for Microsoft.Testing.Platform projects

## Random Number Generation Guidelines
- **Application Code**: Use `Random.Shared.Next()` for performance
- **Test Specimen Builders**: Use `context.Create<int>()` for determinism and reproducible tests
- **Test Determinism**: Configure AutoFixture with seeds when needed for consistent CI/CD results
- **Property-Based Testing**: Prefer controlled randomness over true randomness for reliable test outcomes

## Git and Development Workflow
- Always use descriptive commit messages following conventional commit style
- Include `🤖 Generated with [Claude Code](https://claude.ai/code)` and `Co-Authored-By: Claude <noreply@anthropic.com>` in commits
- Create branches with descriptive names (e.g., `feature/migrate-model-tests-autofixture`)
- When creating PRs, include comprehensive summary of changes and test coverage improvements

## Security and Best Practices
- Never expose or log secrets and keys
- Never commit secrets to the repository
- Always follow security best practices for Alexa skill development
- Validate input constraints according to Alexa specifications

## Testing Migration Strategy
- When migrating existing tests to AutoFixture:
  1. Preserve JSON deserialization tests that validate converters
  2. Replace static object creation tests with property-based AutoFixture tests
  3. Add constraint validation for Alexa-specific business rules
  4. Ensure round-trip serialization testing
  5. Create appropriate specimen builders in TestKit

## Model Testing Patterns
- Use CardSpecimenBuilder for card types
- Generate realistic test data that meets Alexa constraints
- Validate both object structure AND serialization behavior
- Test edge cases and constraint violations