# OpenTelemetry Observability

AlexaVoxCraft provides comprehensive observability through OpenTelemetry instrumentation. The telemetry is **dormant by default** and activates only when you configure OpenTelemetry in your application.

## Installation

Add the observability package to your skill project:

```bash
dotnet add package AlexaVoxCraft.Observability
```

## Basic Setup

```csharp
using AlexaVoxCraft.Observability.Extensions;

// In your Program.cs or Function.cs Init method
services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder.AddAlexaVoxCraftInstrumentation()
               .AddConsoleExporter(); // Your choice of exporter
    })
    .WithMetrics(builder =>
    {
        builder.AddAlexaVoxCraftInstrumentation()
               .AddConsoleExporter(); // Your choice of exporter
    });
```

## Complete Lambda Function Example

```csharp
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

                // Add OpenTelemetry observability
                services.AddOpenTelemetry()
                    .WithTracing(builder =>
                    {
                        builder.AddAlexaVoxCraftInstrumentation()
                               .AddConsoleExporter();
                    })
                    .WithMetrics(builder =>
                    {
                        builder.AddAlexaVoxCraftInstrumentation()
                               .AddConsoleExporter();
                    });
            });
    }
}
```

## What Gets Instrumented

### Spans
- **alexa.lambda.execution**: Overall Lambda execution span with cold start detection
- **alexa.request**: Main request processing span with comprehensive semantic attributes
- **alexa.serialization.request**: Request deserialization span with payload size tracking
- **alexa.serialization.response**: Response serialization span with payload size tracking
- **alexa.handler**: Individual handler execution spans
- **alexa.apl.render**: APL document rendering spans
- Activity events for slot resolutions, response building, and exceptions
- Standard OpenTelemetry exception handling with proper status codes

### Metrics  
- **alexa.requests**: Counter of skill requests by type, intent, locale, device capabilities
- **alexa.latency**: Histogram of request processing time with dimensional tags
- **alexa.handler.duration**: Histogram of handler execution time
- **alexa.serialization.duration**: Histogram of serialization time by direction (request/response)
- **alexa.payload.size**: Histogram of payload sizes in bytes by direction and type
- **alexa.lambda.duration**: Histogram of Lambda execution time
- **alexa.lambda.memory_used**: Histogram of Lambda memory usage in MB
- **alexa.cold_starts**: Counter of Lambda cold starts with `faas.coldstart` attribute
- **alexa.errors**: Counter of errors by classification and context
- **alexa.slot_resolutions**: Counter of slot resolution attempts by status
- **alexa.speech_characters**: Histogram of speech output length for response optimization

### Semantic Attributes (Standards Compliant)

All telemetry follows OpenTelemetry semantic conventions:

**Standard RPC Attributes:**
- `rpc.system`: "alexa"
- `rpc.service`: Application/Skill ID
- `rpc.method`: Intent name or request type

**FaaS Attributes:**
- `faas.coldstart`: true (only on cold starts)

**Standard Exception Events:**
- `exception.type`, `exception.message`, `exception.stacktrace`

**Alexa-Specific Attributes:**
- `alexa.request.type`: LaunchRequest, IntentRequest, etc.
- `alexa.intent.name`: Intent name for IntentRequest
- `alexa.locale`: Request locale (en-US, etc.)
- `alexa.session.new`: Whether this is a new session
- `alexa.device.has_screen`: Device visual capability
- `alexa.dialog.state`: Dialog management state
- `alexa.slot.resolution.status`: Slot resolution results
- `alexa.serialization.direction`: "request" or "response" for serialization operations
- `alexa.payload.size`: Payload size in bytes
- `code.namespace`: Type namespace for serialized objects
- `code.function`: Type name (e.g., "SkillRequest", "SkillResponse")

## Key Benefits

- **Zero Configuration**: Telemetry is dormant by default - no performance impact
- **Standards Compliant**: Full OpenTelemetry semantic conventions
- **Production Ready**: Battle-tested patterns with proper error handling
- **Privacy Safe**: Session correlation uses SHA256 hashing with sufficient entropy
- **Cost Optimized**: Low-cardinality dimensions for cloud metrics compatibility

## Performance Impact

- **Dormant by Default**: Zero overhead when OpenTelemetry is not configured
- **Minimal Overhead**: When active, uses efficient static instrumentation
- **Optimized Hashing**: Session correlation uses proper entropy without excessive computation
- **String Dimensions**: All metric tags are pre-converted to strings for compatibility

## Security & Standards Compliance

The OpenTelemetry implementation follows industry best practices and OpenTelemetry semantic conventions:

**Security Features:**
- **Privacy-Safe Session Correlation**: Uses SHA256 hashing with 32-character entropy for session IDs
- **No Sensitive Data**: Never logs or exports user IDs, access tokens, or PII
- **Error Information**: Exception details are properly structured without exposing internal system details

**Standards Compliance:**
- **OpenTelemetry Semantic Conventions**: Uses standard `rpc.*` and `faas.*` attributes
- **Canonical Exception Events**: Standard `exception.type`, `exception.message`, `exception.stacktrace` fields
- **UCUM Units**: Proper unit specification (e.g., `"By"` for bytes, `"{character}"` for character counts)

**Production Considerations:**
- **Low Cardinality**: Designed to minimize costs with controlled dimension sets
- **Error Classification**: Exceptions are classified by type (validation, business, timeout, unhandled)
- **Proper Activity Status**: Spans are marked with correct `ActivityStatusCode.Ok` or `ActivityStatusCode.Error`

## Next Steps

- [Lambda Hosting](lambda-hosting.md): Learn about AWS Lambda integration
- [Pipeline Behaviors](pipeline-behaviors.md): Understand the request processing pipeline
- [Request Handling](request-handling.md): Build custom request handlers