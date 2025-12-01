# Lambda Hosting

AlexaVoxCraft provides two approaches for hosting Alexa skills in AWS Lambda, both with optimized runtime support, custom serialization, and ReadyToRun publishing capabilities.

> 🎯 **Trivia Skill Examples**: All code examples show deploying a **trivia game skill** to AWS Lambda with DynamoDB integration for storing questions and player scores.

## Overview

Starting with version 5.0.0, AlexaVoxCraft offers two hosting patterns:

- **🌟 Modern Approach (Recommended)**: Minimal API-style hosting using `AlexaVoxCraft.Lambda.Host` powered by [AwsLambda.Host](https://www.nuget.org/packages/AwsLambda.Host)
- **Legacy Approach**: Class-based hosting using `AlexaVoxCraft.MediatR.Lambda` with `AlexaSkillFunction<TRequest, TResponse>`

Both approaches share the same core features and are fully supported. **For new projects, we recommend the modern approach** as it aligns with .NET minimal API patterns and provides more flexibility.

## :rocket: Shared Features

Both hosting approaches provide:

- **:zap: Custom Runtime**: Optimized `provided.al2023` runtime with bootstrap handler
- **:rocket: ReadyToRun Publishing**: Pre-compiled assemblies for faster cold starts
- **:package: Self-Contained Deployment**: No external dependencies required
- **:gear: Custom Serialization**: Alexa-specific JSON serialization with polymorphic support
- **:chart_with_upwards_trend: Observability**: Built-in OpenTelemetry and structured logging
- **:globe_with_meridians: ICU Support**: Internationalization with bundled ICU libraries

## 🌟 Modern Hosting Approach (Recommended)

The modern approach uses the minimal API-style builder pattern familiar from ASP.NET Core, powered by the [AwsLambda.Host](https://www.nuget.org/packages/AwsLambda.Host) package.

### Benefits

- **Familiar Pattern**: Same builder style as ASP.NET Core minimal APIs
- **Industry Standard**: Uses the well-established `AwsLambda.Host` package
- **Flexible Configuration**: Direct access to service collection
- **Better Separation**: Clear separation between infrastructure and business logic
- **Simpler Code**: No need for separate function class

### Installation

```bash
dotnet add package AlexaVoxCraft.Lambda.Host
```

### Basic Setup

#### Program.cs

```csharp
using AlexaVoxCraft.Lambda.Host;
using AlexaVoxCraft.Lambda.Host.Extensions;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AwsLambda.Host.Builder;
using LayeredCraft.Logging.CompactJsonFormatter;
using Microsoft.Extensions.Hosting;
using Sample.Host.Function;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console(new CompactJsonFormatter())
        .CreateBootstrapLogger();

    Log.Information("Starting Lambda Host");

    var builder = LambdaApplication.CreateBuilder();

    // Configure Serilog as the primary logging provider
    builder.Services.AddSerilog(
        (services, lc) =>
            lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );

    // Register MediatR and handlers (auto-discovered at compile time)
    builder.Services.AddSkillMediator(builder.Configuration);

    // Register AlexaVoxCraft hosting services and handler
    builder.Services.AddAlexaSkillHost<LambdaHandler, SkillRequest, SkillResponse>();

    await using var app = builder.Build();

    // Map the Alexa handler
    app.MapHandler(AlexaHandler.Invoke<SkillRequest, SkillResponse>);

    await app.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
```

#### Lambda Handler Implementation

```csharp
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;

namespace Sample.Host.Function;

public class LambdaHandler : ILambdaHandler<SkillRequest, SkillResponse>
{
    private readonly ISkillMediator _mediator;
    private readonly ILogger<LambdaHandler> _logger;

    public LambdaHandler(ISkillMediator mediator, ILogger<LambdaHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillResponse> HandleAsync(SkillRequest request, ILambdaContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received request of type {RequestType}", request.Request.GetType().Name);

        if (request.Request is IntentRequest intent)
        {
            _logger.LogDebug("Received intent {IntentType}", intent.Intent.Name);
        }

        try
        {
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling request");
            throw;
        }
    }
}
```

### Project Configuration

#### Project File (.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AWSProjectType>Lambda</AWSProjectType>
    <AssemblyName>bootstrap</AssemblyName>

    <!-- Performance optimizations -->
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <PublishReadyToRun>true</PublishReadyToRun>

    <!-- Required for AwsLambda.Host interceptors -->
    <InterceptorsNamespaces>$(InterceptorsNamespaces);AwsLambda.Host.Core.Generated</InterceptorsNamespaces>
  </PropertyGroup>

  <ItemGroup>
    <!-- AlexaVoxCraft packages -->
    <PackageReference Include="AlexaVoxCraft.Lambda.Host" Version="5.0.0" />

    <!-- Logging -->
    <PackageReference Include="LayeredCraft.Logging.CompactJsonFormatter" Version="1.0.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
  </ItemGroup>

  <!-- ICU Globalization Support -->
  <ItemGroup>
    <RuntimeHostConfigurationOption Include="System.Globalization.AppLocalIcu" Value="72.1.0.3" />
    <PackageReference Include="Microsoft.ICU.ICU4C.Runtime" Version="72.1.0.3" />
  </ItemGroup>
</Project>
```

### Advanced Configuration with AWS Services

For skills requiring AWS services like DynamoDB:

```csharp
var builder = LambdaApplication.CreateBuilder();

// AWS Configuration
var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();

// MediatR and handlers
builder.Services.AddSkillMediator(builder.Configuration);

// Business services
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();

// Configuration
builder.Services.Configure<DynamoDbOptions>(opt =>
    builder.Configuration.GetSection(DynamoDbOptions.DynamoDbSettings).Bind(opt));

// AlexaVoxCraft hosting and handler registration
builder.Services.AddAlexaSkillHost<LambdaHandler, APLSkillRequest, SkillResponse>();

await using var app = builder.Build();
app.MapHandler(AlexaHandler.Invoke<APLSkillRequest, SkillResponse>);
await app.RunAsync();
```

## Legacy Hosting Approach

The legacy approach uses a class-based pattern with `AlexaSkillFunction<TRequest, TResponse>`. This approach is **fully supported** and ideal for existing projects or teams familiar with this pattern.

### When to Use

- Existing projects already using this pattern
- Teams that prefer class-based architecture
- No need to refactor working code

### Installation

```bash
dotnet add package AlexaVoxCraft.MediatR.Lambda
```

### Basic Setup

#### Program.cs

```csharp
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Response;

APLSupport.Add();

return await LambdaHostExtensions.RunAlexaSkill<TriviaSkillFunction, APLSkillRequest, SkillResponse>(
    handlerBuilder: (function, sp) =>
    {
        var tracer = sp.GetRequiredService<TracerProvider>();
        return (req, ctx) => AWSLambdaWrapper.TraceAsync(tracer, function.FunctionHandlerAsync, req, ctx);
    }
);
```

#### Skill Function Implementation

```csharp
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Hosting;

public class TriviaSkillFunction : AlexaSkillFunction<APLSkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                // AWS Configuration
                var options = context.Configuration.GetAWSOptions();
                services.AddDefaultAWSOptions(options);
                services.AddAWSService<IAmazonDynamoDB>();

                // MediatR and handlers
                services.AddSkillMediator(context.Configuration,
                    cfg => cfg.RegisterServicesFromAssemblyContaining<TriviaSkillFunction>());

                // Business services
                services.AddScoped<IGameRepository, GameRepository>();
                services.AddScoped<IGameService, GameService>();
                services.AddScoped<IVisualBuilder, VisualBuilder>();

                // Configuration
                services.Configure<DynamoDbOptions>(opt =>
                    context.Configuration.GetSection(DynamoDbOptions.DynamoDbSettings).Bind(opt));

                // Observability
                services.AddSingleton(_ => Sdk.CreateTracerProviderBuilder()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("TriviaSkill")
                        .AddTelemetrySdk())
                    .AddAWSInstrumentation()
                    .AddSource("TriviaSkill")
                    .AddOtlpExporter()
                    .AddAWSLambdaConfigurations()
                    .Build());

                // Request decorators
                services.Decorate<IRequestHandler<IntentRequest>, ActivitySourceRequestHandlerDecorator<IntentRequest>>();
                services.Decorate<IRequestHandler<LaunchRequest>, ActivitySourceRequestHandlerDecorator<LaunchRequest>>();
            });
    }
}
```

#### Lambda Handler

The Lambda handler implementation is the same for both approaches:

```csharp
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;

public class LambdaHandler : ILambdaHandler<APLSkillRequest, SkillResponse>
{
    private readonly ISkillMediator _mediator;
    private readonly ILogger<LambdaHandler> _logger;

    public LambdaHandler(ISkillMediator mediator, ILogger<LambdaHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillResponse> HandleAsync(APLSkillRequest request, ILambdaContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received request of type {requestType}", request.Request.GetType().Name);

        if (request.Request is IntentRequest intent)
        {
            _logger.LogDebug("Received intent {intentType}", intent.Intent.Name);
        }

        try
        {
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling request");
            throw;
        }
    }
}
```

### Project Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AWSProjectType>Lambda</AWSProjectType>
    <AssemblyName>bootstrap</AssemblyName>

    <!-- Performance optimizations -->
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <PublishReadyToRun>true</PublishReadyToRun>
  </PropertyGroup>

  <ItemGroup>
    <!-- AlexaVoxCraft packages -->
    <PackageReference Include="AlexaVoxCraft.MediatR.Lambda" Version="5.0.0" />
    <PackageReference Include="AlexaVoxCraft.Model.Apl" Version="5.0.0" />

    <!-- AWS Lambda runtime -->
    <PackageReference Include="Amazon.Lambda.Core" Version="2.6.0" />
    <PackageReference Include="Amazon.Lambda.RuntimeSupport" Version="1.13.1" />

    <!-- AWS services -->
    <PackageReference Include="AWSSDK.DynamoDBv2" Version="4.0.2" />
    <PackageReference Include="AWSSDK.Extensions.NETCore.Setup" Version="4.0.2" />

    <!-- Observability -->
    <PackageReference Include="OpenTelemetry" Version="1.12.0" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.12.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AWS" Version="1.12.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AWSLambda" Version="1.12.0" />

    <!-- Additional dependencies -->
    <PackageReference Include="Scrutor" Version="6.1.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
  </ItemGroup>

  <!-- ICU Globalization Support -->
  <ItemGroup>
    <RuntimeHostConfigurationOption Include="System.Globalization.AppLocalIcu" Value="72.1.0.3" />
    <PackageReference Include="Microsoft.ICU.ICU4C.Runtime" Version="72.1.0.3" />
  </ItemGroup>
</Project>
```

## Shared Configuration

The following configuration applies to both hosting approaches.

### AWS Lambda Tools Configuration

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

### Application Settings

```json
{
  "AWS": {
    "Region": "us-east-1"
  },
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console"
    ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "LayeredCraft.Logging.CompactJsonFormatter.CompactJsonFormatter, LayeredCraft.Logging.CompactJsonFormatter"
        }
      }
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    }
  },
  "SkillConfiguration": {
    "SkillId": "amzn1.ask.skill.your-skill-id",
    "CancellationTimeoutBufferMilliseconds": 500
  },
  "DynamoDbSettings": {
    "TableMaps": {
      "GameRepository": {
        "TableName": "trivia-skill-table",
        "IndexName": "gs1-index"
      }
    }
  }
}
```

## Deployment

### Building and Packaging

```bash
# Install AWS Lambda Tools (one-time setup)
dotnet new tool-manifest --force
dotnet tool install Amazon.Lambda.Tools
dotnet restore

# Create deployment package
dotnet lambda package

# Deploy to AWS
dotnet lambda deploy-function MySkillFunction \
  --function-role arn:aws:iam::123456789012:role/lambda-execution-role \
  --region us-east-1
```

### Package Optimization

```bash
# Self-contained with ReadyToRun
dotnet publish -c Release \
  --self-contained true \
  --runtime linux-x64 \
  -p:PublishReadyToRun=true \
  -p:PublishSingleFile=false
```

### Docker Container Support

```dockerfile
# Dockerfile for container deployment
FROM public.ecr.aws/lambda/dotnet:9-x86_64

# Copy built application
COPY publish/ ${LAMBDA_TASK_ROOT}/

# Set the CMD to your handler
CMD ["bootstrap"]
```

## Performance Optimization

### ReadyToRun Benefits

ReadyToRun compilation provides:
- Faster application startup
- Reduced cold start times
- Better performance for CPU-intensive operations
- Native code generation for frequently used paths

### Memory and Timeout Configuration

```csharp
// Recommended Lambda configuration
Memory: 512 MB - 1024 MB (depending on complexity)
Timeout: 30 seconds (Alexa maximum)
Runtime: provided.al2023
```

### Bundle Size Optimization

```xml
<!-- Enable trimming for smaller packages -->
<PropertyGroup>
  <PublishTrimmed>true</PublishTrimmed>
  <TrimMode>copyused</TrimMode>
</PropertyGroup>

<!-- Preserve necessary assemblies -->
<ItemGroup>
  <TrimmerRootAssembly Include="AlexaVoxCraft.Model" />
  <TrimmerRootAssembly Include="AlexaVoxCraft.Model.Apl" />
</ItemGroup>
```

## Monitoring and Logging

### CloudWatch Integration

```csharp
// CloudWatch-compatible JSON logging
"Serilog": {
  "WriteTo": [
    {
      "Name": "Console",
      "Args": {
        "formatter": "LayeredCraft.Logging.CompactJsonFormatter.CompactJsonFormatter, LayeredCraft.Logging.CompactJsonFormatter"
      }
    }
  ]
}
```

### Request/Response Logging

```csharp
// Enable detailed request/response logging in development
"AlexaVoxCraft.Lambda.Serialization": "Debug"
```

## Security

### IAM Permissions

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents"
      ],
      "Resource": "arn:aws:logs:*:*:*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "dynamodb:GetItem",
        "dynamodb:PutItem",
        "dynamodb:Query",
        "dynamodb:UpdateItem"
      ],
      "Resource": "arn:aws:dynamodb:us-east-1:123456789012:table/trivia-skill-*"
    }
  ]
}
```

### Environment Variables

```csharp
// Secure configuration access
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
var apiKey = Environment.GetEnvironmentVariable("EXTERNAL_API_KEY");
```

## Testing Locally

### AWS Lambda Test Tool

```bash
# Install test tool
dotnet tool install -g Amazon.Lambda.TestTool-9.0

# Run locally
dotnet lambda-test-tool-9.0
```

### Mock Alexa Requests

```csharp
// Test with mock requests
var request = new APLSkillRequest
{
    Version = "1.0",
    Session = new Session
    {
        New = true,
        SessionId = "test-session"
    },
    Request = new LaunchRequest
    {
        RequestId = "test-request",
        Timestamp = DateTimeOffset.UtcNow
    }
};

var response = await handler.HandleAsync(request, mockContext);
```

## Migration Guide

### From Legacy to Modern Hosting

If you're migrating an existing skill from the legacy approach to the modern approach:

#### Step 1: Update Package Reference

```bash
dotnet remove package AlexaVoxCraft.MediatR.Lambda
dotnet add package AlexaVoxCraft.Lambda.Host
```

#### Step 2: Update Project File

Add the interceptors namespace:

```xml
<PropertyGroup>
  <InterceptorsNamespaces>$(InterceptorsNamespaces);AwsLambda.Host.Core.Generated</InterceptorsNamespaces>
</PropertyGroup>
```

#### Step 3: Refactor Program.cs

**Before (Legacy):**
```csharp
return await LambdaHostExtensions.RunAlexaSkill<MySkillFunction, SkillRequest, SkillResponse>();
```

**After (Modern):**
```csharp
var builder = LambdaApplication.CreateBuilder();
builder.Services.AddSkillMediator(builder.Configuration);
builder.Services.AddAlexaSkillHost<LambdaHandler, SkillRequest, SkillResponse>();

await using var app = builder.Build();
app.MapHandler(AlexaHandler.Invoke<SkillRequest, SkillResponse>);
await app.RunAsync();
```

#### Step 4: Remove Function Class

Delete the class that inherited from `AlexaSkillFunction<TRequest, TResponse>` and move its service registration logic directly into Program.cs.

#### Step 5: Update Namespace Imports

If you have direct references to moved classes, update imports:

```csharp
// Change:
using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.Lambda.Serialization;

// To:
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.Lambda.Serialization;
```

### Benefits of Migration

- More familiar pattern for .NET developers
- Easier service configuration
- Better alignment with modern .NET practices
- Simpler Program.cs without nested configuration

### When NOT to Migrate

- Existing skills working well with legacy approach
- Team prefers class-based architecture
- No immediate need for new features
- Migration effort outweighs benefits

## Best Practices

### 1. Use Dependency Injection

```csharp
// Register services properly
services.AddScoped<IMyService, MyService>();
services.AddSingleton<IConfiguration>(configuration);
```

### 2. Handle Cold Starts

```csharp
// Initialize expensive resources outside handler
private static readonly HttpClient HttpClient = new();
private static readonly TracerProvider TracerProvider = CreateTracer();
```

### 3. Implement Proper Cancellation

```csharp
public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    // Use cancellation token in async operations
}
```

### 4. Log Structured Data

```csharp
_logger.LogInformation("Processing {RequestType} for user {UserId}",
    request.GetType().Name,
    userId);
```

## Examples

For complete deployment examples, see the [Examples](../examples/index.md) section with CDK infrastructure and CI/CD pipelines.