# Pipeline Behaviors

AlexaVoxCraft provides a comprehensive pipeline system for implementing cross-cutting concerns through request and response interceptors, enabling clean separation of business logic from infrastructure concerns.

> 🎯 **Trivia Skill Examples**: All code examples show implementing **trivia game infrastructure** including player data loading, game state persistence, performance monitoring, and error handling for quiz interactions.

## :rocket: Features

- **:arrows_counterclockwise: Request/Response Interceptors**: Pre and post-processing pipeline
- **:chart_with_upwards_trend: Observability**: Built-in logging, tracing, and metrics
- **:busts_in_silhouette: User Management**: Authentication and session handling
- **:warning: Error Handling**: Centralized exception processing and recovery
- **:stopwatch: Performance Monitoring**: Request timing and performance metrics
- **:gear: Auto-Registration**: Automatic interceptor discovery and registration

## Basic Usage

### Request Interceptors

```csharp
public class LoggingRequestInterceptor : IRequestInterceptor
{
    private readonly ILogger<LoggingRequestInterceptor> _logger;

    public LoggingRequestInterceptor(ILogger<LoggingRequestInterceptor> logger)
    {
        _logger = logger;
    }

    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = input.RequestEnvelope.Request;
        var userId = input.RequestEnvelope.GetUserId();
        
        _logger.LogInformation("Processing {RequestType} for user {UserId}", 
            request.GetType().Name, 
            userId);

        if (request is IntentRequest intentRequest)
        {
            _logger.LogDebug("Intent: {IntentName} with slots: {@Slots}", 
                intentRequest.Intent.Name, 
                intentRequest.Intent.Slots);
        }

        return Task.CompletedTask;
    }
}
```

### Response Interceptors

```csharp
public class MetricsResponseInterceptor : IResponseInterceptor
{
    private readonly ILogger<MetricsResponseInterceptor> _logger;
    private readonly Counter<int> _responseCounter;

    public MetricsResponseInterceptor(ILogger<MetricsResponseInterceptor> logger)
    {
        _logger = logger;
        _responseCounter = Metrics.CreateCounter<int>("alexa_responses_total", "Total responses sent");
    }

    public Task Process(IHandlerInput input, SkillResponse response, CancellationToken cancellationToken = default)
    {
        var requestType = input.RequestEnvelope.Request.GetType().Name;
        var hasCards = response.Response?.Card != null;
        var hasDirectives = response.Response?.Directives?.Any() == true;

        _responseCounter.Add(1, new KeyValuePair<string, object>[]
        {
            new("request_type", requestType),
            new("has_card", hasCards),
            new("has_directives", hasDirectives)
        });

        _logger.LogDebug("Response sent for {RequestType}: cards={HasCards}, directives={HasDirectives}", 
            requestType, hasCards, hasDirectives);

        return Task.CompletedTask;
    }
}
```

## Game Manager Interceptors

### Request Interceptor for State Loading

From the trivia skill implementation:

```csharp
public class GameServiceRequestInterceptor : IRequestInterceptor
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameServiceRequestInterceptor> _logger;
    private readonly ActivitySource _activitySource = new("TriviaSkill");

    public GameServiceRequestInterceptor(IGameService gameService,
        ILogger<GameServiceRequestInterceptor> logger)
    {
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = input.RequestEnvelope;
        using var activity = DiagnosticsConfig.Source.StartActivityWithTags(
            $"{nameof(GameServiceRequestInterceptor)}.{nameof(Process)}", [
                new(DiagnosticsNames.RpcService, nameof(GameServiceRequestInterceptor)),
                new(DiagnosticsNames.RpcSystem, DiagnosticsConfig.SystemName),
                new(DiagnosticsNames.RequestType, request.Request.Type)
            ]);

        if (request.Request is IntentRequest intent)
        {
            _logger.LogDebug("Received intent {intentType}", intent.Intent.Name);
            activity?.SetTag(DiagnosticsNames.IntentType, intent.Intent.Name);
        }
        
        var userId = input.RequestEnvelope.GetUserId();
        _logger.LogDebug("Loading player for {userId}", userId);
        
        // Load current player and game state in parallel
        var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
        var loadCurrentPlayerTask = _gameService.LoadCurrentPlayer(userId, cancellationToken);
        var loadCurrentGameTask = _gameService.LoadCurrentGame(sessionAttributes, cancellationToken);

        var tasks = new[] { loadCurrentGameTask, loadCurrentPlayerTask };

        try
        {
            await tasks.WhenAll();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (AggregateException ae)
        {
            foreach (var innerException in ae.InnerExceptions)
            {
                _logger.LogError(innerException, innerException.Message);
            }
            activity?.AddException(ae);
            activity?.SetStatus(ActivityStatusCode.Error, ae.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred");
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }
    }
}
```

### Response Interceptor for State Saving

```csharp
public class GameServiceResponseInterceptor : IResponseInterceptor
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameServiceResponseInterceptor> _logger;

    public GameServiceResponseInterceptor(IGameService gameService,
        ILogger<GameServiceResponseInterceptor> logger)
    {
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Process(IHandlerInput input, SkillResponse response, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticsConfig.Source.StartActivityWithTags(
            $"{nameof(GameServiceResponseInterceptor)}.{nameof(Process)}", [
                new(DiagnosticsNames.RpcService, nameof(GameServiceResponseInterceptor)),
                new(DiagnosticsNames.RpcSystem, DiagnosticsConfig.SystemName)
            ]);

        try
        {
            // Save game state to session attributes
            var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
            await _gameService.SaveCurrentGame(sessionAttributes, cancellationToken);
            await input.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);

            // Save player data to persistent storage
            await _gameService.SaveCurrentPlayer(cancellationToken);
            
            _logger.LogDebug("Successfully saved game and player state");
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving game state");
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }
    }
}
```

## Localization Interceptors

### Request Interceptor for Localization

```csharp
public class LocalizationRequestInterceptor : IRequestInterceptor
{
    private readonly ILogger<LocalizationRequestInterceptor> _logger;

    public LocalizationRequestInterceptor(ILogger<LocalizationRequestInterceptor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var locale = input.RequestEnvelope.Request.Locale ?? "en-US";
        
        // Set culture for string resources
        var culture = new CultureInfo(locale);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        _logger.LogDebug("Set locale to {locale}", locale);
        
        // Store locale in request context for later use
        input.RequestEnvelope.Context ??= new Context();
        input.RequestEnvelope.Context.Extensions ??= new Dictionary<string, object>();
        input.RequestEnvelope.Context.Extensions["locale"] = locale;
        
        return Task.CompletedTask;
    }
}
```

## Activity Source Decorators

### Request Handler Decorator

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
        using var activity = _activitySource.StartActivityWithTags($"{_decorated.GetType().Name}.Handle", [
            new(DiagnosticsNames.RpcService, _decorated.GetType().Name),
            new(DiagnosticsNames.RpcSystem, DiagnosticsConfig.SystemName),
            new(DiagnosticsNames.RequestType, typeof(TRequest).Name),
            new(DiagnosticsNames.UserId, handlerInput.RequestEnvelope.GetUserId())
        ]);

        if (handlerInput.RequestEnvelope.Request is IntentRequest intent)
        {
            activity?.SetTag(DiagnosticsNames.IntentType, intent.Intent.Name);
        }

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

// Service registration with Scrutor
services.Decorate<IRequestHandler<IntentRequest>, ActivitySourceRequestHandlerDecorator<IntentRequest>>();
services.Decorate<IRequestHandler<LaunchRequest>, ActivitySourceRequestHandlerDecorator<LaunchRequest>>();
services.Decorate<IRequestHandler<UserEventRequest>, ActivitySourceRequestHandlerDecorator<UserEventRequest>>();
services.Decorate<IRequestHandler<SessionEndedRequest>, ActivitySourceRequestHandlerDecorator<SessionEndedRequest>>();
```

## Diagnostics Configuration

### Activity Source Setup

```csharp
public static class DiagnosticsConfig
{
    public static readonly ActivitySource Source = new("TriviaSkill");
    public static readonly string ServiceName = "TriviaSkill";
    public static readonly string SystemName = "AlexaVoxCraft";
    
    // Metrics
    public static readonly Counter<int> CorrectCounter = 
        Metrics.CreateCounter<int>("trivia_correct_answers", "Number of correct answers");
    
    public static readonly Counter<int> IncorrectCounter = 
        Metrics.CreateCounter<int>("trivia_incorrect_answers", "Number of incorrect answers");
    
    public static readonly Histogram<double> RequestDuration = 
        Metrics.CreateHistogram<double>("alexa_request_duration", "Request processing duration");
}

public static class DiagnosticsNames
{
    public const string RpcService = "rpc.service";
    public const string RpcSystem = "rpc.system";
    public const string RequestType = "alexa.request.type";
    public const string IntentType = "alexa.intent.name";
    public const string UserId = "alexa.user.id";
}
```

### Extension Methods for Activities

```csharp
public static class ActivityExtensions
{
    public static Activity? StartActivityWithTags(this ActivitySource source, string name, 
        KeyValuePair<string, object?>[] tags)
    {
        var activity = source.StartActivity(name);
        if (activity != null)
        {
            foreach (var tag in tags)
            {
                activity.SetTag(tag.Key, tag.Value);
            }
        }
        return activity;
    }

    public static void AddException(this Activity? activity, Exception exception)
    {
        if (activity == null) return;
        
        activity.SetTag("exception.type", exception.GetType().FullName);
        activity.SetTag("exception.message", exception.Message);
        activity.SetTag("exception.stacktrace", exception.StackTrace);
    }
}
```

## Performance Monitoring

### Request Timing Interceptor

```csharp
public class PerformanceRequestInterceptor : IRequestInterceptor
{
    private readonly ILogger<PerformanceRequestInterceptor> _logger;
    private readonly Histogram<double> _requestDuration;

    public PerformanceRequestInterceptor(ILogger<PerformanceRequestInterceptor> logger)
    {
        _logger = logger;
        _requestDuration = Metrics.CreateHistogram<double>("alexa_request_duration_seconds", "Request processing duration");
    }

    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        input.RequestEnvelope.Context ??= new Context();
        input.RequestEnvelope.Context.Extensions ??= new Dictionary<string, object>();
        input.RequestEnvelope.Context.Extensions["performance_stopwatch"] = stopwatch;
        
        return Task.CompletedTask;
    }
}

public class PerformanceResponseInterceptor : IResponseInterceptor
{
    private readonly ILogger<PerformanceResponseInterceptor> _logger;
    private readonly Histogram<double> _requestDuration;

    public PerformanceResponseInterceptor(ILogger<PerformanceResponseInterceptor> logger)
    {
        _logger = logger;
        _requestDuration = Metrics.CreateHistogram<double>("alexa_request_duration_seconds", "Request processing duration");
    }

    public Task Process(IHandlerInput input, SkillResponse response, CancellationToken cancellationToken = default)
    {
        if (input.RequestEnvelope.Context?.Extensions?.TryGetValue("performance_stopwatch", out var stopwatchObj) == true &&
            stopwatchObj is Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var duration = stopwatch.Elapsed.TotalSeconds;
            var requestType = input.RequestEnvelope.Request.GetType().Name;
            
            _requestDuration.Record(duration, new KeyValuePair<string, object>[]
            {
                new("request_type", requestType)
            });

            if (duration > 5.0) // Log slow requests
            {
                _logger.LogWarning("Slow request detected: {RequestType} took {Duration:F2}s", 
                    requestType, duration);
            }
            else
            {
                _logger.LogDebug("Request {RequestType} completed in {Duration:F2}s", 
                    requestType, duration);
            }
        }

        return Task.CompletedTask;
    }
}
```

## Error Handling Pipeline

### Global Exception Handler

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly Counter<int> _errorCounter;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
        _errorCounter = Metrics.CreateCounter<int>("alexa_errors_total", "Total errors");
    }

    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        return Task.FromResult(true); // Handle all exceptions
    }

    public Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        var requestType = handlerInput.RequestEnvelope.Request.GetType().Name;
        var userId = handlerInput.RequestEnvelope.GetUserId();
        
        _errorCounter.Add(1, new KeyValuePair<string, object>[]
        {
            new("request_type", requestType),
            new("exception_type", ex.GetType().Name)
        });

        _logger.LogError(ex, "Unhandled exception in {RequestType} for user {UserId}: {Message}", 
            requestType, userId, ex.Message);

        var speechText = ex switch
        {
            TimeoutException => "Sorry, that took too long. Please try again.",
            ArgumentException => "Sorry, there was a problem with your request. Please try again.",
            InvalidOperationException => "Sorry, I can't do that right now. Please try again later.",
            _ => "Sorry, something went wrong. Please try again."
        };

        return handlerInput.ResponseBuilder
            .Speak(speechText)
            .Reprompt("Is there anything else I can help you with?")
            .GetResponse(cancellationToken);
    }
}
```

## Task Extension Utilities

### Parallel Task Execution

```csharp
public static class TaskExtensions
{
    public static async Task WhenAll(this Task[] tasks)
    {
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
            // Examine each task to see which ones faulted
            var exceptions = new List<Exception>();
            foreach (var task in tasks)
            {
                if (task.IsFaulted && task.Exception != null)
                {
                    exceptions.AddRange(task.Exception.InnerExceptions);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
            throw;
        }
    }

    public static async Task<T[]> WhenAll<T>(this Task<T>[] tasks)
    {
        try
        {
            return await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
            var exceptions = new List<Exception>();
            var results = new List<T>();
            
            foreach (var task in tasks)
            {
                if (task.IsFaulted && task.Exception != null)
                {
                    exceptions.AddRange(task.Exception.InnerExceptions);
                }
                else if (task.IsCompletedSuccessfully)
                {
                    results.Add(task.Result);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
            
            return results.ToArray();
        }
    }
}
```

## Registration and Configuration

### Automatic Registration

```csharp
// In your AlexaSkillFunction
protected override void Init(IHostBuilder builder)
{
    builder.ConfigureServices((context, services) =>
    {
        // Auto-register interceptors from assembly
        services.AddSkillMediator(context.Configuration, cfg => 
            cfg.RegisterServicesFromAssemblyContaining<Program>());

        // Manual interceptor registration (if needed)
        services.AddScoped<IRequestInterceptor, GameServiceRequestInterceptor>();
        services.AddScoped<IRequestInterceptor, LocalizationRequestInterceptor>();
        services.AddScoped<IRequestInterceptor, PerformanceRequestInterceptor>();
        
        services.AddScoped<IResponseInterceptor, GameServiceResponseInterceptor>();
        services.AddScoped<IResponseInterceptor, MetricsResponseInterceptor>();
        services.AddScoped<IResponseInterceptor, PerformanceResponseInterceptor>();

        // Exception handlers
        services.AddScoped<IExceptionHandler, GlobalExceptionHandler>();

        // Decorator registration with Scrutor
        services.Decorate<IRequestHandler<IntentRequest>, ActivitySourceRequestHandlerDecorator<IntentRequest>>();
        services.Decorate<IRequestHandler<LaunchRequest>, ActivitySourceRequestHandlerDecorator<LaunchRequest>>();
    });
}
```

### Conditional Registration

```csharp
// Environment-specific interceptors
if (context.HostingEnvironment.IsDevelopment())
{
    services.AddScoped<IRequestInterceptor, DebugRequestInterceptor>();
    services.AddScoped<IResponseInterceptor, DebugResponseInterceptor>();
}

if (context.Configuration.GetValue<bool>("EnableDetailedMetrics"))
{
    services.AddScoped<IResponseInterceptor, DetailedMetricsInterceptor>();
}
```

## Best Practices

### 1. Keep Interceptors Focused

Each interceptor should have a single responsibility:

```csharp
// ✅ Good - Single responsibility
public class AuthenticationInterceptor : IRequestInterceptor { }
public class LoggingInterceptor : IRequestInterceptor { }

// ❌ Avoid - Multiple responsibilities
public class AuthenticationAndLoggingInterceptor : IRequestInterceptor { }
```

### 2. Handle Exceptions Gracefully

```csharp
public async Task Process(IHandlerInput input, CancellationToken cancellationToken)
{
    try
    {
        // Interceptor logic
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in {InterceptorName}", GetType().Name);
        // Don't rethrow unless critical - let the request continue
    }
}
```

### 3. Use Structured Logging

```csharp
_logger.LogInformation("Processing {RequestType} for user {UserId} in {Duration}ms", 
    requestType, 
    userId, 
    stopwatch.ElapsedMilliseconds);
```

### 4. Implement Proper Cancellation

```csharp
public async Task Process(IHandlerInput input, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    
    await _service.DoWorkAsync(cancellationToken);
    
    cancellationToken.ThrowIfCancellationRequested();
}
```

## Examples

For complete pipeline behavior examples, see the [Examples](../examples/index.md) section with the trivia skill's comprehensive interceptor implementation.