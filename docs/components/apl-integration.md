# APL Integration

AlexaVoxCraft provides comprehensive support for Alexa Presentation Language (APL), enabling rich visual interfaces with synchronized voice and touch interactions through a fluent document builder API.

> 🎯 **Trivia Skill Examples**: All code examples demonstrate building **visual trivia questions** with multiple-choice answers, score displays, and interactive touch/voice responses.

## :rocket: Features

- **:art: Fluent Document Builder**: Intuitive API for creating complex APL documents
- **:arrows_counterclockwise: Multi-Slide Presentations**: Sequential slide management with voice synchronization
- **:point_up_2: Interactive Components**: Touch-enabled elements with event handling
- **:speaker: Voice-Touch Sync**: Coordinated audio and visual experiences
- **:wrench: Custom Layouts**: Reusable layout components and transformers
- **:iphone: Device Adaptive**: Responsive design for different screen sizes

## Basic Usage

### Document Builder Setup

```csharp
// Register DocumentBuilder service
services.AddScoped<IVisualBuilder, VisualBuilder>();

// In your handler
public class MyHandler : IRequestHandler<LaunchRequest>
{
    private readonly IVisualBuilder _visualBuilder;

    public MyHandler(IVisualBuilder visualBuilder)
    {
        _visualBuilder = visualBuilder;
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        // Create APL document
        var (renderDirective, executeDirective) = _visualBuilder
            .AddTextSlide("Welcome to Trivia Challenge!", "Say 'start' to begin")
            .GetDirectives();

        return await input.ResponseBuilder
            .AddDirective(renderDirective)
            .AddDirective(executeDirective)
            .Speak("Welcome to Trivia Challenge! Ready to start?")
            .GetResponse(cancellationToken);
    }
}
```

### Simple Text Slide

```csharp
var (renderDirective, executeDirective) = _visualBuilder
    .AddTextSlide("Game Over! Your score was 4 out of 5.")
    .GetDirectives();

input.ResponseBuilder
    .AddDirective(renderDirective)
    .AddDirective(executeDirective);
```

## Document Builder API

### Text Slides

Create informational slides with speech synchronization:

```csharp
// Basic text slide
_visualBuilder.AddTextSlide("Welcome to Trivia Challenge!");

// Text slide with footer hint
_visualBuilder.AddTextSlide(
    speechText: "Correct! That's the right answer.",
    footerText: "Say 'next' for the next question"
);

// Multiple text slides
var (renderDirective, executeDirective) = _visualBuilder
    .AddTextSlide("Round 1 Complete!", "show me the scores")
    .AddTextSlide($"Your score: {currentScore} out of {totalQuestions}", "ready for round 2?")
    .GetDirectives();
```

### Question Slides

Interactive multiple-choice questions:

```csharp
var questionText = "What is the capital of France?";
var choices = new[] { "London", "Berlin", "Paris", "Madrid" };

var (renderDirective, executeDirective) = _visualBuilder
    .AddQuestionSlide(
        speechText: $"Question 1: {questionText}",
        questionText: questionText,
        choices: choices,
        footerText: "Select an answer or say the number"
    )
    .GetDirectives();
```

### Complete Visual Builder Example

Generic implementation for skill visual interfaces:

```csharp
public class VisualBuilder : IVisualBuilder
{
    private readonly RenderDocumentDirective _renderDocumentDirective;
    private readonly ExecuteCommandsDirective _executeCommandsDirective;
    private readonly APLDocument _document;
    private readonly List<APLCommand> _commands;
    private readonly Dictionary<string, Layout> _layouts;
    private readonly List<APLComponent> _components;
    private readonly Dictionary<string, object> _properties;
    private readonly List<APLTransformer> _transformers;

    public VisualBuilder()
    {
        _commands = new List<APLCommand>();
        _layouts = new Dictionary<string, Layout>();
        _components = new List<APLComponent>();
        _properties = new Dictionary<string, object>();
        _transformers = new List<APLTransformer>();

        // Create main document structure
        _document = new APLDocument(APLDocumentVersion.V2022_2)
        {
            Theme = ViewportTheme.Dark,
            Imports = new List<Import> { Import.AlexaLayouts },
            Resources = Resources.GetSkillResources(),
            Layouts = _layouts,
            MainTemplate = new Layout(new Container
            {
                Width = "100vw",
                Height = "100vh",
                Items = new List<APLComponent>
                {
                    new Pager
                    {
                        Id = "skillPager",
                        Navigation = "wrap",
                        Height = "100%",
                        Width = "100%",
                        Items = _components
                    }
                }
            })
        };

        // Setup data sources and directives
        _renderDocumentDirective = new RenderDocumentDirective
        {
            Token = "skillToken",
            DataSources = new Dictionary<string, APLDataSource>
            {
                {
                    "skillData", new ObjectDataSource
                    {
                        ObjectId = "skillData",
                        TopLevelData = new Dictionary<string, object>
                        {
                            { "backgroundImage", "https://example.com/background.jpg" },
                            { "titleText", "Trivia Challenge" },
                            { "logoUrl", "https://example.com/logo.png" }
                        },
                        Properties = _properties,
                        Transformers = _transformers
                    }
                }
            },
            Document = _document
        };
    }

    public IVisualBuilder AddTextSlide(string speechText, string? footerText = null)
    {
        _layouts["TextSlide"] = Layouts.TextSlideLayout;
        
        var pageId = $"page{_components.Count + 1}";
        AddSpeechCommand(pageId);

        _components.Add(new CustomComponent("TextSlide")
        {
            Properties = new Dictionary<string, object>
            {
                { "backgroundColor", "" },
                { "backgroundImageSource", "${payload.skillData.backgroundImage}" },
                { "title", "${payload.skillData.titleText}" },
                { "logoUrl", "${payload.skillData.logoUrl}" },
                { "text", $"${{payload.skillData.properties.{pageId}.speechText}}" },
                { "speech", $"${{payload.skillData.properties.{pageId}.speechTextAsSpeech}}" },
                { "hintText", $"${{payload.skillData.properties.{pageId}.hintText}}" },
                { "speechId", pageId }
            }
        });

        var items = new Dictionary<string, object>
        {
            { "speechText", $"<speak>{speechText}</speak>" }
        };

        if (!string.IsNullOrWhiteSpace(footerText))
        {
            items["footerHintText"] = footerText;
            _transformers.Add(new APLTransformer("textToHint", $"{pageId}.footerHintText", "hintText"));
        }

        _properties[pageId] = items;
        _transformers.Add(new APLTransformer("ssmlToSpeech", $"{pageId}.speechText", "speechTextAsSpeech"));
        
        return this;
    }

    public IVisualBuilder AddQuestionSlide(string speechText, string questionText, 
        IEnumerable<string> choices, string? footerText = null)
    {
        _layouts["MultipleChoiceItem"] = Layouts.MultipleChoiceItemLayout;
        _layouts["MultipleChoice"] = Layouts.MultipleChoiceLayout;
        
        var pageId = $"page{_components.Count + 1}";
        AddSpeechCommand(pageId);

        _components.Add(new CustomComponent("MultipleChoice")
        {
            Properties = new Dictionary<string, object>
            {
                { "backgroundColor", "" },
                { "backgroundImageSource", "${payload.skillData.backgroundImage}" },
                { "title", "${payload.skillData.titleText}" },
                { "logoUrl", "${payload.skillData.logoUrl}" },
                { "text", $"${{payload.skillData.properties.{pageId}.speechText}}" },
                { "speech", $"${{payload.skillData.properties.{pageId}.speechTextAsSpeech}}" },
                { "hintText", $"${{payload.skillData.properties.{pageId}.hintText}}" },
                { "primaryText", $"${{payload.skillData.properties.{pageId}.primaryText}}" },
                { "choices", $"${{payload.skillData.properties.{pageId}.choices}}" },
                { "choiceListType", $"${{payload.skillData.properties.{pageId}.choiceListType}}" },
                { "speechId", pageId }
            }
        });

        var items = new Dictionary<string, object>
        {
            { "speechText", $"<speak>{speechText}</speak>" },
            { "primaryText", questionText },
            { "choices", choices },
            { "choiceListType", "ordinal" }
        };

        if (!string.IsNullOrWhiteSpace(footerText))
        {
            items["footerHintText"] = footerText;
            _transformers.Add(new APLTransformer("textToHint", $"{pageId}.footerHintText", "hintText"));
        }

        _properties[pageId] = items;
        _transformers.Add(new APLTransformer("ssmlToSpeech", $"{pageId}.speechText", "speechTextAsSpeech"));
        
        return this;
    }

    private void AddSpeechCommand(string componentId)
    {
        if (_commands.Any())
        {
            _commands.Add(new SetPage
            {
                ComponentId = "skillPager",
                Position = SetPagePosition.Relative,
                DelayMilliseconds = 100,
                Value = 1
            });
        }
        
        _commands.Add(new SpeakItem { ComponentId = componentId });
    }

    public (RenderDocumentDirective?, ExecuteCommandsDirective?) GetDirectives()
    {
        _executeCommandsDirective.Commands = new List<APLCommand>
        {
            new Sequential { Commands = _commands }
        };

        return (_renderDocumentDirective, _executeCommandsDirective);
    }
}
```

## Layout Components

### Text Slide Layout

```csharp
public static class Layouts
{
    public static Layout TextSlideLayout => new Layout(
        new Container
        {
            Width = "100vw",
            Height = "100vh",
            AlignItems = "center",
            JustifyContent = "center",
            Items = new List<APLComponent>
            {
                new Image
                {
                    Source = "${backgroundImageSource}",
                    Scale = "bestFill",
                    Width = "100vw",
                    Height = "100vh",
                    Position = "absolute"
                },
                new Container
                {
                    Width = "100vw",
                    Height = "100vh",
                    AlignItems = "center",
                    JustifyContent = "spaceBetween",
                    PaddingTop = "@marginTop",
                    PaddingBottom = "@marginBottom",
                    Items = new List<APLComponent>
                    {
                        new Header(),
                        new Text
                        {
                            Text = "${text}",
                            Style = "textStyleBody",
                            TextAlign = "center",
                            MaxLines = 8
                        },
                        new Footer()
                    }
                }
            }
        })
    {
        Description = "Text slide with background image and header",
        Parameters = new List<Parameter>
        {
            "backgroundColor",
            "backgroundImageSource", 
            "title",
            "logoUrl",
            "text",
            "hintText"
        }
    };

    public static Layout MultipleChoiceLayout => new Layout(
        new Container
        {
            // Multiple choice layout implementation
        })
    {
        Description = "Interactive multiple choice question",
        Parameters = new List<Parameter>
        {
            "backgroundColor",
            "backgroundImageSource",
            "title", 
            "logoUrl",
            "primaryText",
            "choices",
            "choiceListType",
            "hintText"
        }
    };
}
```

## Interactive Events

### Touch Event Handling

```csharp
public class AnswerHandler : IRequestHandler<UserEventRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(input.RequestEnvelope.Request is UserEventRequest);
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        var userEvent = (UserEventRequest)input.RequestEnvelope.Request;
        var answer = GetAnswerFromEvent(userEvent.Arguments);
        
        // Process the touch-selected answer
        var isCorrect = await _gameManager.UpdateScore(answer, cancellationToken);
        
        // Continue with response...
        return await BuildResponse(isCorrect, input, cancellationToken);
    }

    private int GetAnswerFromEvent(object[]? arguments)
    {
        if (arguments == null || arguments.Length <= 1)
            return 0;

        // Check for "answer" event type
        if (!arguments.Any(arg => arg is string str && str == "answer"))
            return 0;

        // Extract answer value (1-4)
        if (arguments[1] is int answer && answer is > 0 and <= 4)
            return answer;

        return 0;
    }
}
```

### APL Commands

```csharp
// Sequential commands for slide transitions
var commands = new List<APLCommand>
{
    new SetPage
    {
        ComponentId = "pagerId",
        Position = SetPagePosition.Relative,
        Value = 1,
        DelayMilliseconds = 100
    },
    new SpeakItem
    {
        ComponentId = "questionSlide"
    },
    new Parallel
    {
        Commands = new List<APLCommand>
        {
            new AnimateItem
            {
                ComponentId = "choices",
                Duration = 1000,
                Value = new List<AnimatedProperty>
                {
                    new AnimatedProperty { Property = "opacity", To = 1 }
                }
            }
        }
    }
};
```

## Data Sources and Transformers

### Object Data Source

```csharp
var dataSource = new ObjectDataSource
{
    ObjectId = "triviaData",
    TopLevelData = new Dictionary<string, object>
    {
        { "backgroundImage", "https://example.com/bg.jpg" },
        { "titleText", "Trivia Challenge" },
        { "logoUrl", "https://example.com/logo.png" }
    },
    Properties = _properties,
    Transformers = _transformers
};
```

### Data Transformers

```csharp
// Transform SSML to speech-ready text
_transformers.Add(new APLTransformer(
    "ssmlToSpeech", 
    $"{pageId}.speechText", 
    "speechTextAsSpeech"
));

// Transform footer text to hint format
_transformers.Add(new APLTransformer(
    "textToHint", 
    $"{pageId}.footerHintText", 
    "hintText"
));
```

## Device Support

### APL Capability Check

```csharp
public static class SkillRequestExtensions
{
    public static bool APLSupported(this SkillRequest request)
    {
        return request.Context?.System?.Device?.SupportedInterfaces?.ContainsKey("Alexa.Presentation.APL") == true;
    }
}

// Usage in handlers
if (input.RequestEnvelope.APLSupported())
{
    var (renderDirective, executeDirective) = _documentBuilder
        .AddQuestionSlide(speechText, questionText, choices)
        .GetDirectives();
        
    input.ResponseBuilder
        .AddDirective(renderDirective)
        .AddDirective(executeDirective);
}
```

### Responsive Design

```csharp
// Different layouts for different screen sizes
var layout = viewport.Shape == ViewportShape.Round 
    ? Layouts.RoundScreenLayout 
    : Layouts.RectangleScreenLayout;
```

## Best Practices

### 1. Progressive Enhancement

Always provide voice-only fallbacks:

```csharp
var speechText = "What's your answer?";
var repromptText = "Say a number from 1 to 4, or say 'I don't know'.";

if (input.RequestEnvelope.APLSupported())
{
    // Add visual components
    var (renderDirective, executeDirective) = _documentBuilder
        .AddQuestionSlide(speechText, questionText, choices)
        .GetDirectives();
    // ...
}

return await input.ResponseBuilder
    .Speak(speechText)
    .Reprompt(repromptText)
    .GetResponse(cancellationToken);
```

### 2. Coordinate Voice and Visual

Ensure speech matches visual content:

```csharp
var speechText = "Question 2: Who created Mickey Mouse? Your choices are: 1. Walt Disney, 2. Ub Iwerks, 3. Roy Disney, 4. Carl Barks.";
var questionText = "Who created Mickey Mouse?";
var choices = new[] { "Walt Disney", "Ub Iwerks", "Roy Disney", "Carl Barks" };

_documentBuilder.AddQuestionSlide(speechText, questionText, choices);
```

### 3. Handle Both Voice and Touch

Support multiple interaction modes:

```csharp
public class AnswerHandler : IRequestHandler<IntentRequest>, IRequestHandler<UserEventRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            input.RequestEnvelope.Request is UserEventRequest ||
            (input.RequestEnvelope.Request is IntentRequest intent && 
             intent.Intent.Name == "AnswerIntent"));
    }
}
```

### 4. Use Meaningful IDs

```csharp
var pageId = $"question{questionNumber}";
var componentId = $"choices{questionNumber}";
```

## Examples

For complete APL integration examples, see the [Examples](../examples/index.md) section with the trivia skill's visual interface implementation.