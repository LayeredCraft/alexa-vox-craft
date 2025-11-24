# Request Handling

AlexaVoxCraft provides a powerful request handling system built on MediatR patterns, enabling CQRS-style request processing with type safety, auto-discovery, and comprehensive pipeline support.

> 🎯 **Trivia Skill Examples**: All code examples in this documentation demonstrate building a **trivia game skill** where users answer multiple-choice questions to test their knowledge.

## :rocket: Features

- **:gear: MediatR Integration**: Built on proven CQRS patterns with MediatR
- **:zap: Source Generation**: Compile-time handler registration with zero runtime reflection
- **:shield: Type Safety**: Compile-time request/response validation
- **:arrows_counterclockwise: Pipeline Behaviors**: Composable request/response processing
- **:warning: Exception Handling**: Centralized error handling and recovery
- **:chart_with_upwards_trend: Observability**: Built-in logging and telemetry support

## Basic Usage

### Handler Registration

Handlers are automatically discovered and registered at compile time using source generation:

```csharp
// In your AlexaSkillFunction
protected override void Init(IHostBuilder builder)
{
    builder
        .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
        .ConfigureServices((context, services) =>
        {
            // Handlers are automatically discovered and registered at compile time
            services.AddSkillMediator(context.Configuration);
        });
}
```

See [Source Generation](source-generation.md) for detailed information about compile-time handler registration, customization with attributes, and performance benefits.

### Simple Request Handler

```csharp
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

public class LaunchRequestHandler : IRequestHandler<LaunchRequest>
{
    public bool CanHandle(IHandlerInput handlerInput) => 
        handlerInput.RequestEnvelope.Request is LaunchRequest;

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return await input.ResponseBuilder
            .Speak("Welcome to my skill!")
            .WithShouldEndSession(false)
            .GetResponse(cancellationToken);
    }
}
```

## Handler Types

### Launch Request Handler

Handles skill launch and start-over intents:

```csharp
public class LaunchRequestHandler : IRequestHandler<LaunchRequest>, IRequestHandler<IntentRequest>
{
    private readonly IGameService _gameService;
    private readonly IVisualBuilder _visualBuilder;

    public LaunchRequestHandler(IGameService gameService, IVisualBuilder visualBuilder)
    {
        _gameService = gameService;
        _visualBuilder = visualBuilder;
    }

    public Task<bool> CanHandle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handlerInput.RequestEnvelope.Request is LaunchRequest ||
                               (handlerInput.RequestEnvelope.Request is IntentRequest intent &&
                                intent.Intent.Name == BuiltInIntent.StartOver));
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
        
        await _gameService.StartNewGame(cancellationToken);
        
        var speechOutput = "Welcome to Trivia Challenge! Ready to test your knowledge?";
        var repromptText = "Say yes to start, or help for instructions.";

        if (input.RequestEnvelope.APLSupported())
        {
            var (renderDirective, executeDirective) = _visualBuilder
                .AddWelcomeSlide(speechOutput, "show me the high scores")
                .GetDirectives();
                
            input.ResponseBuilder
                .AddDirective(renderDirective)
                .AddDirective(executeDirective);
        }

        return await input.ResponseBuilder
            .Speak(speechOutput)
            .Reprompt(repromptText)
            .WithSimpleCard("Trivia Challenge", speechOutput)
            .GetResponse(cancellationToken);
    }
}
```

### Intent Request Handler

Handles specific intents with slot processing:

```csharp
public class AnswerHandler : IRequestHandler<IntentRequest>, IRequestHandler<UserEventRequest>
{
    private readonly IGameService _gameService;
    private readonly ILogger<AnswerHandler> _logger;

    public AnswerHandler(IGameService gameService, ILogger<AnswerHandler> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(input.RequestEnvelope.Request is UserEventRequest ||
                               input.RequestEnvelope.Request is IntentRequest intent &&
                               (intent.Intent.Name == "AnswerIntent" ||
                                intent.Intent.Name == "DontKnowIntent" ||
                                intent.Intent.Name == BuiltInIntent.Next));
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        var submittedAnswer = GetAnswer(input.RequestEnvelope.Request);
        var result = await _gameService.ProcessAnswer(submittedAnswer, cancellationToken);
        
        var speechOutput = result.IsCorrect 
            ? "Correct! Well done." 
            : $"Sorry, that's incorrect. The correct answer was {result.CorrectAnswer}.";

        if (result.IsGameComplete)
        {
            // Game over logic
            speechOutput += $" Game over! Your final score is {result.FinalScore} out of {result.TotalQuestions}.";
            
            return await input.ResponseBuilder
                .Speak(speechOutput)
                .WithShouldEndSession(true)
                .GetResponse(cancellationToken);
        }

        // Continue with next question
        var nextQuestion = await _gameService.GetNextQuestion(cancellationToken);
        var questionPrompt = BuildQuestionPrompt(nextQuestion);
        
        return await input.ResponseBuilder
            .Speak(speechOutput + " " + questionPrompt)
            .Reprompt(questionPrompt)
            .GetResponse(cancellationToken);
    }

    private int GetAnswer(Request request)
    {
        return request switch
        {
            IntentRequest intentRequest => GetAnswerFromSlot(intentRequest.Intent),
            UserEventRequest eventRequest => GetAnswerFromEvent(eventRequest.Arguments),
            _ => 0
        };
    }

    private int GetAnswerFromSlot(Intent? intent)
    {
        if (intent?.Slots == null || !intent.Slots.ContainsKey("Answer"))
            return 0;
            
        if (int.TryParse(intent.Slots["Answer"].Value, out var answer) && answer is > 0 and <= 4)
            return answer;
            
        return 0;
    }

    private string BuildQuestionPrompt(QuestionData question)
    {
        var prompt = $"Question {question.Number}: {question.Text} ";
        for (int i = 0; i < question.Choices.Count; i++)
        {
            prompt += $"{i + 1}. {question.Choices[i]}. ";
        }
        return prompt;
    }
}
```

### Base Handler Pattern

Create base handlers for common functionality:

```csharp
public abstract class BaseGameHandler
{
    protected const int GameLength = 5;
    protected const int AnswerCount = 4;
    
    protected readonly ILogger Logger;
    protected readonly IGameService GameService;
    protected readonly IVisualBuilder VisualBuilder;

    protected BaseGameHandler(ILogger logger, IGameService gameService, IVisualBuilder visualBuilder)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        GameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        VisualBuilder = visualBuilder ?? throw new ArgumentNullException(nameof(visualBuilder));
    }

    public abstract Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default);
    public abstract Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default);

    protected async Task<SkillResponse> StartGame(bool isNewGame, IHandlerInput handlerInput, CancellationToken cancellationToken)
    {
        var sessionAttributes = await handlerInput.AttributesManager.GetSessionAttributes(cancellationToken);
        var speechOutput = new StringBuilder();
        
        if (isNewGame)
        {
            speechOutput.AppendFormat("Welcome to {0}! ", "Trivia Challenge");
            speechOutput.AppendFormat("I'll ask you {0} questions. ", GameLength);
        }

        var gameState = await GameService.StartNewGame(cancellationToken);
        var questionPrompt = BuildQuestionPrompt(gameState.CurrentQuestion);
        speechOutput.Append(questionPrompt);
        
        sessionAttributes["speechOutput"] = questionPrompt;
        sessionAttributes["repromptText"] = questionPrompt;

        await handlerInput.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);
        
        return await handlerInput.ResponseBuilder
            .Speak(speechOutput.ToString())
            .Reprompt(questionPrompt)
            .WithSimpleCard("Trivia Challenge", questionPrompt)
            .GetResponse(cancellationToken);
    }

    private string BuildQuestionPrompt(QuestionData question)
    {
        var prompt = new StringBuilder();
        prompt.AppendFormat("Question {0}: {1} ", 
            question.Number, 
            question.Text);
            
        for (int i = 0; i < question.Choices.Count; i++)
        {
            prompt.AppendFormat("{0}. {1}. ", i + 1, question.Choices[i]);
        }
        
        return prompt.ToString();
    }
}

// Usage in derived handlers
public class LaunchRequestHandler : BaseGameHandler, IRequestHandler<LaunchRequest>
{
    public LaunchRequestHandler(ILogger<LaunchRequestHandler> logger, 
        IGameService gameService, 
        IVisualBuilder visualBuilder) 
        : base(logger, gameService, visualBuilder)
    {
    }

    public override Task<bool> CanHandle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handlerInput.RequestEnvelope.Request is LaunchRequest);
    }

    public override Task<SkillResponse> Handle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        return StartGame(true, handlerInput, cancellationToken);
    }
}
```

## Built-in Handlers

### Session Ended Handler

```csharp
public class SessionEndedHandler : IRequestHandler<SessionEndedRequest>
{
    private readonly IGameService _gameService;
    private readonly ILogger<SessionEndedHandler> _logger;

    public SessionEndedHandler(IGameService gameService, ILogger<SessionEndedHandler> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public Task<bool> CanHandle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handlerInput.RequestEnvelope.Request is SessionEndedRequest);
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Session ended: {reason}", 
            ((SessionEndedRequest)input.RequestEnvelope.Request).Reason);
            
        // Save any pending game state
        await _gameService.SaveGameProgress(cancellationToken);
        
        return await input.ResponseBuilder.GetResponse(cancellationToken);
    }
}
```

### Exception Handler

```csharp
public class ErrorHandler : IExceptionHandler
{
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(ILogger<ErrorHandler> logger)
    {
        _logger = logger;
    }

    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        return Task.FromResult(true); // Handle all exceptions
    }

    public Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogError(ex, "Unhandled exception in skill");
        
        var speechText = "Sorry, I had trouble doing what you asked. Please try again.";
        
        return handlerInput.ResponseBuilder
            .Speak(speechText)
            .Reprompt(speechText)
            .GetResponse(cancellationToken);
    }
}
```

## Request Processing Pipeline

### Request Flow

1. **Lambda Entry**: Request enters through `LambdaHandler.HandleAsync`
2. **Request Interceptors**: Pre-processing (authentication, logging, state loading)
3. **Handler Resolution**: MediatR resolves appropriate handler based on `CanHandle` logic
4. **Handler Execution**: Handler processes request and generates response
5. **Response Interceptors**: Post-processing (state saving, cleanup, telemetry)
6. **Response Return**: Final response sent to Alexa

### Lambda Handler Implementation

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

    public async Task<SkillResponse> HandleAsync(APLSkillRequest request, ILambdaContext context, CancellationToken cancellationToken)
    { 
        using var activity = Activity.StartActivity($"{nameof(LambdaHandler)}.{nameof(HandleAsync)}");
        
        if (request.Request is IntentRequest intent)
        {
            _logger.LogDebug("Received intent {intentType}", intent.Intent.Name);
            activity?.SetTag("alexa.intent.name", intent.Intent.Name);
        }

        _logger.LogDebug("Received request of type {requestType}", request.Request.Type);

        try
        {
            var response = await _mediator.Send(request, cancellationToken);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling request");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
```

## Handler Registration

### Source-Generated Registration (Default)

Handlers are automatically discovered and registered at compile time using C# interceptors:

```csharp
// Compile-time registration - no runtime reflection
services.AddSkillMediator(context.Configuration);
```

Handlers are registered as `Transient` by default (new instance per request), which is appropriate for stateless handlers. You can customize the lifetime using the `[AlexaHandler]` attribute.

All handlers implementing `IRequestHandler<T>`, `IDefaultRequestHandler`, `IExceptionHandler`, or related interfaces are automatically registered. See the [Source Generation](source-generation.md) documentation for:

- Customizing service lifetimes with `[AlexaHandler(Lifetime = ServiceLifetime.Scoped)]`
- Controlling execution order with `[AlexaHandler(Order = 10)]`
- Excluding handlers from registration with `[AlexaHandler(Exclude = true)]`
- Performance benefits (80% faster Lambda cold starts)

### Runtime Registration (Legacy)

If source generation is disabled via `<EnableMediatRGeneratorInterceptor>false</EnableMediatRGeneratorInterceptor>`, you must use runtime assembly scanning:

```csharp
// ⚠️ Only when source generation is disabled
services.AddSkillMediator(context.Configuration, cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());
```

## Best Practices

### 1. Use Dependency Injection

```csharp
public class MyHandler : IRequestHandler<LaunchRequest>
{
    private readonly IMyService _service;
    private readonly ILogger<MyHandler> _logger;

    public MyHandler(IMyService service, ILogger<MyHandler> logger)
    {
        _service = service;
        _logger = logger;
    }
}
```

### 2. Implement Proper Cancellation

```csharp
public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    
    var data = await _service.GetDataAsync(cancellationToken);
    
    return await input.ResponseBuilder
        .Speak("Response")
        .GetResponse(cancellationToken);
}
```

### 3. Use Structured Logging

```csharp
_logger.LogDebug("Processing {requestType} for user {userId}", 
    request.GetType().Name, 
    input.RequestEnvelope.GetUserId());
```

### 4. Handle Slot Validation

```csharp
private string? GetSlotValue(Intent intent, string slotName)
{
    if (intent.Slots?.TryGetValue(slotName, out var slot) == true)
    {
        return slot.Value;
    }
    return null;
}
```

## Default Voice Configuration

AlexaVoxCraft supports configuring a default Amazon Polly voice that automatically wraps all speech output. This allows you to give your skill a consistent voice personality without modifying every handler.

### Configuration

Set the default voice in your skill configuration:

```csharp
// In your AlexaSkillFunction
protected override void Init(IHostBuilder builder)
{
    builder
        .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
        .ConfigureServices((context, services) =>
        {
            services.AddSkillMediator(context.Configuration);

            // Configure default voice
            services.Configure<SkillServiceConfiguration>(config =>
            {
                config.DefaultVoiceName = AlexaSupportedVoices.EnglishUS.Matthew;
            });
        });
}
```

Or via `appsettings.json`:

```json
{
  "SkillConfiguration": {
    "SkillId": "amzn1.ask.skill.your-skill-id",
    "DefaultVoiceName": "Matthew"
  }
}
```

### How It Works

When a default voice is configured, all `Speak()` and `Reprompt()` calls automatically wrap their content in an SSML `<voice>` element:

```csharp
// Your code
input.ResponseBuilder.Speak("Welcome to Trivia Challenge!");

// Generated SSML (with DefaultVoiceName = "Matthew")
<speak><voice name="Matthew">Welcome to Trivia Challenge!</voice></speak>
```

This works with both plain text and SSML elements:

```csharp
// Plain text
input.ResponseBuilder.Speak("Hello world");

// SSML elements
input.ResponseBuilder.Speak(new PlainText("Hello"), new Audio("sound.mp3"));
```

### Available Voices

Use the `AlexaSupportedVoices` class for voice constants that are guaranteed to work with Alexa Skills:

```csharp
using AlexaVoxCraft.Model.Response.Ssml;

// English (US)
AlexaSupportedVoices.EnglishUS.Ivy       // Female
AlexaSupportedVoices.EnglishUS.Joanna    // Female
AlexaSupportedVoices.EnglishUS.Kendra    // Female
AlexaSupportedVoices.EnglishUS.Kimberly  // Female
AlexaSupportedVoices.EnglishUS.Salli     // Female
AlexaSupportedVoices.EnglishUS.Joey      // Male
AlexaSupportedVoices.EnglishUS.Justin    // Male
AlexaSupportedVoices.EnglishUS.Matthew   // Male

// English (UK)
AlexaSupportedVoices.EnglishGB.Amy       // Female
AlexaSupportedVoices.EnglishGB.Emma      // Female
AlexaSupportedVoices.EnglishGB.Brian     // Male

// English (Australia)
AlexaSupportedVoices.EnglishAU.Nicole    // Female
AlexaSupportedVoices.EnglishAU.Russell   // Male

// German (Germany)
AlexaSupportedVoices.GermanDE.Marlene    // Female
AlexaSupportedVoices.GermanDE.Vicki      // Female
AlexaSupportedVoices.GermanDE.Hans       // Male

// And many more locales...
```

See the [Alexa SSML Reference](https://developer.amazon.com/en-US/docs/alexa/custom-skills/speech-synthesis-markup-language-ssml-reference.html#supported-amazon-polly-voices) for the complete list of supported voices.

### Manual Voice Wrapping

You can also wrap specific content in a voice using the `WithVoice()` extension methods:

```csharp
using AlexaVoxCraft.MediatR.Response;

// Wrap plain text
var speech = "Hello world".WithVoice(AlexaSupportedVoices.EnglishUS.Joanna);

// Wrap SSML elements
var ssml = new PlainText("Hello").WithVoice(AlexaSupportedVoices.EnglishGB.Amy);

// Wrap multiple elements
var elements = new ISsml[] { new PlainText("Hello"), new Break(BreakStrength.Medium) };
var voiced = elements.WithVoice(AlexaSupportedVoices.EnglishUS.Matthew);
```

### Trivia Skill Example

Give your trivia skill a distinct personality with a consistent voice:

```csharp
public class TriviaSkillFunction : AlexaSkillFunction<APLSkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration);

                // Use Matthew's voice for a friendly male host
                services.Configure<SkillServiceConfiguration>(config =>
                {
                    config.DefaultVoiceName = AlexaSupportedVoices.EnglishUS.Matthew;
                });
            });
    }
}
```

Now all responses automatically use Matthew's voice:

```csharp
// In any handler - voice wrapping happens automatically
return await input.ResponseBuilder
    .Speak("Welcome to Trivia Challenge! Ready to test your knowledge?")
    .Reprompt("Say yes to start playing, or help for instructions.")
    .GetResponse(cancellationToken);
```

## Examples

For more real-world examples, see the [Examples](../examples/index.md) section which includes the complete Disney Trivia skill implementation.