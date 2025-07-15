# Lambda Hosting

AlexaVoxCraft provides optimized AWS Lambda hosting with custom serialization, ReadyToRun publishing, and comprehensive deployment support for Alexa skills.

> 🎯 **Trivia Skill Examples**: All code examples show deploying a **trivia game skill** to AWS Lambda with DynamoDB integration for storing questions and player scores.

## :rocket: Features

- **:zap: Custom Runtime**: Optimized `provided.al2023` runtime with bootstrap handler
- **:rocket: ReadyToRun Publishing**: Pre-compiled assemblies for faster cold starts
- **:package: Self-Contained Deployment**: No external dependencies required
- **:gear: Custom Serialization**: Alexa-specific JSON serialization with polymorphic support
- **:chart_with_upwards_trend: Observability**: Built-in OpenTelemetry and structured logging
- **:globe_with_meridians: ICU Support**: Internationalization with bundled ICU libraries

## Basic Setup

### Function Configuration

```csharp
// Program.cs
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

### Skill Function Implementation

```csharp
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

## Project Configuration

### Project File Setup

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
    <LangVersion>default</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <!-- Core AlexaVoxCraft packages -->
    <PackageReference Include="AlexaVoxCraft.MediatR.Lambda" Version="2.0.0.61" />
    <PackageReference Include="AlexaVoxCraft.Model.Apl" Version="2.0.0.61" />
    
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

### AWS Lambda Tools Configuration

```json
// aws-lambda-tools-defaults.json
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

## Lambda Handler Implementation

### Core Handler

```csharp
public class LambdaHandler : ILambdaHandler<APLSkillRequest, SkillResponse>
{
    private readonly ISkillMediator _mediator;
    private readonly ILogger<LambdaHandler> _logger;

    public LambdaHandler(ISkillMediator mediator, ILogger<LambdaHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillResponse> HandleAsync(APLSkillRequest request, ILambdaContext context)
    { 
        using var activity = DiagnosticsConfig.Source.StartActivityWithTags(
            $"{nameof(LambdaHandler)}.{nameof(HandleAsync)}", new()
            {
                new("rpc.service", nameof(LambdaHandler)),
                new("rpc.system", "AlexaVoxCraft"),
                new("request.type", request.Request.GetType().Name)
            });

        if (request.Request is IntentRequest intent)
        {
            _logger.LogDebug("Received intent {intentType}", intent.Intent.Name);
            activity?.SetTag("alexa.intent.name", intent.Intent.Name);
        }

        _logger.LogDebug("Received request of type {requestType}", request.Request.Type);

        try
        {
            var response = await _mediator.Send(request);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling request");
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
```

### Activity Source Decorator

```csharp
public class ActivitySourceRequestHandlerDecorator<TRequest> : IRequestHandler<TRequest>
    where TRequest : Request
{
    private readonly IRequestHandler<TRequest> _decorated;
    private readonly ActivitySource _activitySource;

    public ActivitySourceRequestHandlerDecorator(IRequestHandler<TRequest> decorated)
    {
        _decorated = decorated;
        _activitySource = new ActivitySource("TriviaSkill");
    }

    public Task<bool> CanHandle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        return _decorated.CanHandle(handlerInput, cancellationToken);
    }

    public async Task<SkillResponse> Handle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity($"{_decorated.GetType().Name}.Handle");
        
        activity?.SetTag("alexa.request.type", typeof(TRequest).Name);
        activity?.SetTag("alexa.user.id", handlerInput.RequestEnvelope.GetUserId());

        try
        {
            var result = await _decorated.Handle(handlerInput, cancellationToken);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
```

## Configuration

### Application Settings

```json
// appsettings.json
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

### Environment-Specific Configuration

```csharp
// Configuration helper
public static class ConfigurationExtensions
{
    public static void ConfigureForEnvironment(this IServiceCollection services, 
        IConfiguration configuration, string environment)
    {
        switch (environment.ToLower())
        {
            case "development":
                services.Configure<DynamoDbOptions>(opt =>
                {
                    opt.TableMaps["GameRepository"].TableName = "trivia-skill-dev";
                });
                break;
                
            case "production":
                services.Configure<DynamoDbOptions>(opt =>
                {
                    opt.TableMaps["GameRepository"].TableName = "trivia-skill-prod";
                });
                break;
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

### Custom Metrics

```csharp
public class DiagnosticsConfig
{
    public static readonly ActivitySource Source = new("TriviaSkill");
    public static readonly string ServiceName = "TriviaSkill";
    public static readonly string SystemName = "AlexaVoxCraft";
    
    public static readonly Counter<int> CorrectCounter = 
        Metrics.CreateCounter<int>("trivia_correct_answers", "Number of correct answers");
    
    public static readonly Counter<int> IncorrectCounter = 
        Metrics.CreateCounter<int>("trivia_incorrect_answers", "Number of incorrect answers");
}

// Usage in handlers
DiagnosticsConfig.CorrectCounter.Add(1, new KeyValuePair<string, object>[]
{
    new("user.id", userId),
    new("question.category", "general")
});
```

### Request/Response Logging

```csharp
// Enable detailed request/response logging in development
"AlexaVoxCraft.MediatR.Lambda.Serialization": "Debug"
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