# Examples - Complete Trivia Skill Implementation

Real-world usage examples and patterns for AlexaVoxCraft, featuring a **complete trivia game skill** implementation that demonstrates all major framework capabilities.

> 🎯 **About the Trivia Skill**: This documentation showcases building a full-featured **trivia challenge game** where users answer multiple-choice questions, earn points, and compete on leaderboards. All code examples are production-ready and can be adapted for your own quiz-based skills.

## Complete Skill Examples

### Trivia Challenge Skill

A comprehensive, production-ready trivia game that showcases all AlexaVoxCraft features:

- **Game Logic**: Question management, scoring, and leaderboards
- **Visual Interface**: APL documents with interactive slides  
- **Data Persistence**: DynamoDB integration for user and game data
- **Observability**: OpenTelemetry tracing and structured logging
- **Deployment**: AWS Lambda with CDK infrastructure

```csharp
// Complete skill implementation
public class TriviaSkillFunction : AlexaSkillFunction<APLSkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                // AWS Services
                var options = context.Configuration.GetAWSOptions();
                services.AddDefaultAWSOptions(options);
                services.AddAWSService<IAmazonDynamoDB>();

                // Core framework
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
                        .AddService(DiagnosticsConfig.ServiceName)
                        .AddTelemetrySdk())
                    .AddAWSInstrumentation()
                    .AddSource(DiagnosticsConfig.ServiceName)
                    .AddOtlpExporter()
                    .AddAWSLambdaConfigurations()
                    .Build());

                // Request decorators for tracing
                services.Decorate<IRequestHandler<IntentRequest>, ActivitySourceRequestHandlerDecorator<IntentRequest>>();
                services.Decorate<IRequestHandler<LaunchRequest>, ActivitySourceRequestHandlerDecorator<LaunchRequest>>();
                services.Decorate<IRequestHandler<UserEventRequest>, ActivitySourceRequestHandlerDecorator<UserEventRequest>>();
                services.Decorate<IRequestHandler<SessionEndedRequest>, ActivitySourceRequestHandlerDecorator<SessionEndedRequest>>();
            });
    }
}
```

## Handler Implementation Patterns

### Launch Request Handler

```csharp
public class LaunchRequestHandler : BaseGameHandler, IRequestHandler<LaunchRequest>, IRequestHandler<IntentRequest>
{
    public LaunchRequestHandler(ILogger<LaunchRequestHandler> logger, 
        IGameService gameService,
        IVisualBuilder visualBuilder) 
        : base(logger, gameService, visualBuilder)
    {
    }

    public override Task<bool> CanHandle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Checking for Can Handle in {handler}", nameof(LaunchRequestHandler));
        return Task.FromResult(handlerInput.RequestEnvelope.Request is LaunchRequest ||
                               (handlerInput.RequestEnvelope.Request is IntentRequest intent &&
                                intent.Intent.Name == BuiltInIntent.StartOver));
    }

    public override Task<SkillResponse> Handle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Handling in {handler}", nameof(LaunchRequestHandler));
        return StartGame(true, handlerInput, cancellationToken);
    }
}
```

### Complex Intent Handler

```csharp
public class AnswerHandler : BaseGameHandler, IRequestHandler<IntentRequest>, IRequestHandler<UserEventRequest>
{
    private readonly KeyValuePair<string, object>[] _counterAttributes =
    [
        new("signal", "metric"), 
        new("language", "dotnet"), 
        new("metricType", "request")
    ];

    public AnswerHandler(ILogger<AnswerHandler> logger, 
        IGameService gameService,
        IVisualBuilder visualBuilder) 
        : base(logger, gameService, visualBuilder)
    {
    }

    public override Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Checking for Can Handle in {handler}", nameof(AnswerHandler));
        return Task.FromResult(input.RequestEnvelope.Request is UserEventRequest ||
                               input.RequestEnvelope.Request is IntentRequest intent &&
                               (intent.Intent.Name == CustomIntent.Answer ||
                                intent.Intent.Name == CustomIntent.DontKnow ||
                                intent.Intent.Name == BuiltInIntent.Next));
    }

    public override async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Handling in {handler}", nameof(AnswerHandler));
        
        if ((input.RequestEnvelope.Request as IntentRequest)?.Intent.Name == CustomIntent.Answer)
        {
            return await HandleUserGuess(false, input, cancellationToken);
        }

        return await HandleUserGuess(true, input, cancellationToken);
    }

    private async Task<SkillResponse> HandleUserGuess(bool userGaveUp, IHandlerInput input, CancellationToken cancellationToken)
    {
        var submittedAnswer = GetAnswer(input.RequestEnvelope.Request);
        var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
        var answerCorrect = await GameService.UpdateScore(submittedAnswer, cancellationToken);
        
        var speechOutput = new StringBuilder();
        var speechOutputAnalysis = new StringBuilder();
        var aplFirstPageSpeechOutput = new StringBuilder();
        var aplSecondPageSpeechOutput = new StringBuilder();
        
        if (answerCorrect)
        {
            DiagnosticsConfig.CorrectCounter.Add(1, _counterAttributes);
            speechOutputAnalysis.Append("Correct! Well done.");
        }
        else
        {
            DiagnosticsConfig.IncorrectCounter.Add(1, _counterAttributes);
            if (!userGaveUp)
            {
                speechOutputAnalysis.Append("Sorry, that's incorrect. ");
            }
            speechOutputAnalysis.AppendFormat("The correct answer was {0}. ", CurrentGame.CorrectAnswerText);
        }

        // Game over logic
        if (CurrentGame.CurrentQuestionIndex >= GameLength - 1)
        {
            aplFirstPageSpeechOutput.AppendFormat("{0}{1}", speechOutput, speechOutputAnalysis);
            aplSecondPageSpeechOutput.AppendFormat("Game over! Your final score: {0} out of {1}. ", 
                CurrentGame.CurrentScore, GameLength);

            if (!userGaveUp)
            {
                speechOutput.Append("The answer is ");
            }

            speechOutput.Append($"{speechOutputAnalysis}{aplSecondPageSpeechOutput}");

            if (input.RequestEnvelope.APLSupported())
            {
                speechOutput.Clear();
                var (renderDocumentDirective, executeCommandsDirective) = VisualBuilder
                    .AddTextSlide(aplFirstPageSpeechOutput.ToString())
                    .AddTextSlide(aplSecondPageSpeechOutput.ToString())
                    .GetDirectives();

                input.ResponseBuilder
                    .AddDirective(renderDocumentDirective!)
                    .AddDirective(executeCommandsDirective!);
            }

            return await input.ResponseBuilder
                .Speak(speechOutput.ToString())
                .WithShouldEndSession(true)
                .GetResponse(cancellationToken);
        }

        // Continue game - load next question
        await GameService.LoadNextQuestion(cancellationToken);
        
        var repromptText = new StringBuilder();
        repromptText.AppendFormat("Question {0}: {1} ", CurrentGame.CurrentQuestionIndex + 1, CurrentGame.QuestionText);
        
        var i = 0;
        foreach (var answer in CurrentGame.Answers)
        {
            i++;
            repromptText.Append($"{i}. {answer}. ");
        }

        if (!userGaveUp)
        {
            speechOutput.Append("The answer is ");
        }

        aplFirstPageSpeechOutput.Append($"{speechOutput}{speechOutputAnalysis}Your score: {CurrentGame.CurrentScore}. ");
        aplSecondPageSpeechOutput.Append(repromptText.ToString());
        speechOutput.Append($"{speechOutputAnalysis}Your score: {CurrentGame.CurrentScore}. {repromptText}");

        sessionAttributes[Attributes.SpeechOutput] = repromptText.ToString();
        sessionAttributes[Attributes.RepromptText] = repromptText.ToString();

        if (input.RequestEnvelope.APLSupported())
        {
            speechOutput.Clear();
            var (renderDocumentDirective, executeCommandsDirective) = VisualBuilder
                .AddTextSlide(aplFirstPageSpeechOutput.ToString(), "show me the high scores")
                .AddQuestionSlide(repromptText.ToString(), CurrentGame.QuestionText, CurrentGame.Answers)
                .GetDirectives();

            input.ResponseBuilder
                .AddDirective(renderDocumentDirective!)
                .AddDirective(executeCommandsDirective!);
        }

        await input.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);

        return await input.ResponseBuilder
            .Speak(speechOutput.ToString())
            .Reprompt(repromptText.ToString())
            .WithSimpleCard("Trivia Challenge", repromptText.ToString())
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
        if (intent?.Slots == null || !intent.Slots.ContainsKey("Answer") || 
            string.IsNullOrEmpty(intent.Slots["Answer"].Value)) 
            return 0;
            
        if (int.TryParse(intent.Slots["Answer"].Value, out var answer) && 
            answer is < AnswerCount + 1 and > 0)
        {
            return answer;
        }
        return 0;
    }

    private int GetAnswerFromEvent(object[]? arguments)
    {
        if (arguments == null || arguments.Length <= 1)
            return 0;

        // Check if "answer" is present in the array
        if (!arguments.Any(arg => arg is string str && str == "answer"))
            return 0;

        // Ensure the second item is an int
        if (arguments[1] is int answer && answer is > 0 and <= AnswerCount)
        {
            return answer;
        }
        return 0;
    }
}
```

## Base Handler Pattern

### Shared Game Logic

```csharp
public abstract class BaseGameHandler
{
    protected const int GameLength = GameControls.GameLength;
    protected const int AnswerCount = GameControls.AnswerCount;
    protected readonly ILogger Logger;
    protected readonly IGameService GameService;
    protected readonly GameState CurrentGame;
    protected readonly IVisualBuilder VisualBuilder;

    public abstract Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default);
    public abstract Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default);

    protected BaseGameHandler(ILogger logger, IGameService gameService, IVisualBuilder visualBuilder)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        GameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        VisualBuilder = visualBuilder ?? throw new ArgumentNullException(nameof(visualBuilder));
        CurrentGame = GameService.CurrentGame;
    }

    protected async Task<SkillResponse> StartGame(bool newGame, IHandlerInput handlerInput, CancellationToken cancellationToken)
    {
        var sessionAttributes = await handlerInput.AttributesManager.GetSessionAttributes(cancellationToken);
        var speechOutput = new StringBuilder();
        
        if (newGame)
        {
            speechOutput.AppendFormat("Welcome to {0}! ", "Trivia Challenge");
            speechOutput.AppendFormat("I'll ask you {0} questions. ", GameLength);
        }

        var aplFirstPageSpeechOutput = new StringBuilder(speechOutput.ToString());

        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug("CurrentGame before starting: {@gameData}", CurrentGame);
        }
        
        await GameService.StartNewGame(cancellationToken);

        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug("CurrentGame after starting: {@gameData}", CurrentGame);
        }
        
        var spokenQuestion = CurrentGame.QuestionText;
        var repromptText = new StringBuilder();
        repromptText.AppendFormat("Question {0}: {1} ", 1, spokenQuestion);
        
        var i = 0;
        foreach (var answer in CurrentGame.Answers)
        {
            i++;
            repromptText.Append($"{i}. {answer}. ");
        }

        speechOutput.Append(repromptText.ToString());
        
        sessionAttributes[Attributes.SpeechOutput] = repromptText.ToString();
        sessionAttributes[Attributes.RepromptText] = repromptText.ToString();

        if (handlerInput.RequestEnvelope.APLSupported())
        {
            speechOutput.Clear();
            var (renderDocumentDirective, executeCommandsDirective) = VisualBuilder
                .AddTextSlide(aplFirstPageSpeechOutput.ToString(), "show me the high scores")
                .AddQuestionSlide(repromptText.ToString(), spokenQuestion, CurrentGame.Answers, "the answer is 1")
                .GetDirectives();
                
            handlerInput.ResponseBuilder
                .AddDirective(renderDocumentDirective!)
                .AddDirective(executeCommandsDirective!);
        }

        await handlerInput.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);
            
        return await handlerInput.ResponseBuilder
            .Speak(speechOutput.ToString())
            .Reprompt(repromptText.ToString())
            .WithSimpleCard("Trivia Challenge", repromptText.ToString())
            .GetResponse(cancellationToken);
    }
}
```

## Service Layer Examples

### Game Service

```csharp
public class GameService : IGameService
{
    private const int GameLength = GameControls.GameLength;
    private const int AnswerCount = GameControls.AnswerCount;
    private const int GameBuckets = GameControls.GameBuckets;
    
    private readonly ILogger<GameService> _logger;
    private readonly IGameRepository _gameRepository;
    private readonly Random _random;

    public Player? CurrentPlayer { get; private set; }
    public GameState CurrentGame { get; private set; }

    public GameService(ILogger<GameService> logger, IGameRepository gameRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _random = Random.Shared;
        CurrentGame = new GameState
        {
            CorrectAnswerText = string.Empty,
            QuestionText = string.Empty
        };
    }

    public async Task StartNewGame(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting a new game");
        var gameQuestions = await PopulateGameQuestions();
        var correctAnswerIndex = (int)Math.Floor(_random.NextDouble() * AnswerCount);
        var (bucket, id) = gameQuestions.First();
        var question = await GetQuestion(bucket, id, cancellationToken);

        var roundAnswers = PopulateRoundAnswers(question, correctAnswerIndex).ToList();

        CurrentGame.GameQuestions = gameQuestions;
        CurrentGame.CorrectAnswerIndex = correctAnswerIndex;
        CurrentGame.QuestionText = question.Message;
        CurrentGame.Answers = roundAnswers.Select(answer => answer.Message);
        CurrentGame.CurrentQuestionIndex = 0;
        CurrentGame.CurrentScore = 0;
        CurrentGame.CorrectAnswerText = question.Answers[0].Message;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("New game started with values {@newGame}", CurrentGame);
        }
    }

    public async Task LoadCurrentPlayer(string userId, CancellationToken cancellationToken)
    {
        Player? player = null;
        try
        {
            player = await _gameRepository.GetPlayerAsync(userId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to load current player: {message}", exception.Message);
        }
        
        if (player is null)
        {
            var currentDate = DateTimeOffset.UtcNow;
            player = new Player
            {
                UserId = userId,
                Username = $"Player{userId.AsciiToInt()}",
                CorrectAnswers = 0,
                TotalAnswers = 0,
                Score = 0,
                PercentageCorrect = 0,
                CreatedDate = currentDate,
                UpdatedDate = currentDate,
                PlayerType = "active"
            };
            await _gameRepository.SavePlayerAsync(player, cancellationToken);
        }

        CurrentPlayer = player;
    }

    public Task<bool> UpdateScore(int submittedAnswer, CancellationToken cancellationToken)
    {
        var answerCorrect = submittedAnswer - 1 == (CurrentGame?.CorrectAnswerIndex ?? -1);
        if (CurrentPlayer is not null)
        {
            CurrentPlayer.TotalAnswers++;
            if (answerCorrect)
            {
                CurrentPlayer.CorrectAnswers++;
                CurrentGame!.CurrentScore++;
            }

            CurrentPlayer.PercentageCorrect = (double)CurrentPlayer.CorrectAnswers / CurrentPlayer.TotalAnswers * 100;
            CurrentPlayer.Score = (CurrentPlayer.CorrectAnswers * 100) + (int)(CurrentPlayer.PercentageCorrect * 10);
            CurrentPlayer.UpdatedDate = DateTimeOffset.UtcNow;
            CurrentPlayer.PlayerType = "active";
        }

        return Task.FromResult(answerCorrect);
    }

    private async Task<Dictionary<int, Guid>> PopulateGameQuestions()
    {
        var count = await GetTableCount();
        if (GameLength > count)
        {
            throw new ArgumentOutOfRangeException(nameof(GameLength), "Invalid Game Length.");
        }
        
        return Enumerable.Range(1, GameBuckets)
            .OrderBy(_ => Guid.NewGuid()) // Shuffle bucket numbers
            .Take(GameLength)             // Select random buckets
            .ToDictionary(                // Pair with random UUIDs
                bucket => bucket, 
                _ => Guid.NewGuid()
            );
    }

    private IEnumerable<Answer> PopulateRoundAnswers(Question question, int correctAnswerIndex)
    {
        var index = question.Answers.Count;
        if (index < AnswerCount)
        {
            throw new ArgumentOutOfRangeException(nameof(AnswerCount), "Not enough answers for question.");
        }

        var answers = new Answer[AnswerCount];
        var answersCopy = new Answer[AnswerCount];
        question.Answers.CopyTo(answersCopy);
        
        // Fisher-Yates shuffle
        for (var i = 1; i < answersCopy.Length; i++)
        {
            var rand = ((int)Math.Floor(_random.NextDouble() * (index - 1))) + 1;
            index--;
            (answersCopy[index], answersCopy[rand]) = (answersCopy[rand], answersCopy[index]);
        }

        for (var i = 0; i < AnswerCount; i++)
        {
            answers[i] = answersCopy[i];
        }

        // Place correct answer at specified index
        (answers[0], answers[correctAnswerIndex]) = (answers[correctAnswerIndex], answers[0]);

        return answers;
    }
}
```

### Repository Pattern

```csharp
public class GameRepository : IGameRepository
{
    private readonly IAmazonDynamoDB _client;
    private readonly ILogger<GameRepository> _logger;
    private readonly DynamoDbTableMap _options;

    public GameRepository(IAmazonDynamoDB client, ILogger<GameRepository> logger, IOptions<DynamoDbOptions> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        var repositoryName = GetType().Name;
        if (!options.Value.TableMaps.TryGetValue(repositoryName, out var map))
            throw new ArgumentException($"{repositoryName} not found in table maps", nameof(repositoryName));
        _options = map;
    }

    public async Task<Question> GetQuestionAsync(int bucket, Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new QueryRequest
            {
                TableName = _options.TableName,
                IndexName = _options.IndexName,
                KeyConditionExpression = "#gs1pk = :pk AND #gs1sk >= :randomId",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#gs1pk", "gs1-pk" },
                    { "#gs1sk", "gs1-sk" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":pk", new AttributeValue { S = $"game#trivia-skill#bucket#{bucket}#" } },
                    { ":randomId", new AttributeValue { S = $"key#{id}" } }
                },
                Limit = 1,
                ScanIndexForward = true
            };
            
            var response = await _client.QueryAsync(request, cancellationToken);
            if (response.Items.Count == 0)
            {
                _logger.LogDebug("No question found, flipping ScanIndexForward");
                request.ScanIndexForward = !request.ScanIndexForward;
                response = await _client.QueryAsync(request, cancellationToken);
            }
            
            if (response.Items.Count == 0)
            {
                _logger.LogDebug("No question found");
                throw new ArgumentException($"Question {bucket} not found", nameof(bucket));
            }
            
            return Question.CreateFromAttributes(response.Items[0]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception retrieving question {QuestionId}", bucket);
            throw;
        }
    }

    public async Task<IEnumerable<Player>> RetrieveTopPlayers(int numberOfPlayers = 5, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Looking for the top {topPlayers} players", numberOfPlayers);
        try
        {
            var request = new QueryRequest
            {
                TableName = _options.TableName,
                IndexName = _options.IndexName,
                Limit = numberOfPlayers,
                ScanIndexForward = false, // Descending order (highest scores first)
                KeyConditionExpression = "#gs1pk = :pk",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#gs1pk", "gs1-pk" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":pk", new AttributeValue { S = "game#trivia-skill#player-type#active" } }
                }
            };

            var response = await _client.QueryAsync(request, cancellationToken);
            return response.Items.Select(Player.CreateFromAttributes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving top players");
            return [];
        }
    }
}
```

## APL Visual Builder

### Complete Visual Builder Implementation

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

        // Create main document with pager container
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
                        Id = ComponentIds.pagerId,
                        Navigation = "wrap",
                        Height = "100%",
                        Width = "100%",
                        Items = _components
                    }
                }
            })
            {
                Description = "Trivia Game",
                Parameters = new List<Parameter> { "payload" }
            }
        };

        // Setup data sources
        _renderDocumentDirective = new RenderDocumentDirective
        {
            Token = Constants.TOKEN_ID,
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

        _executeCommandsDirective = new ExecuteCommandsDirective
        {
            Token = Constants.TOKEN_ID,
            Commands = new List<APLCommand>
            {
                new Sequential { Commands = _commands }
            }
        };
    }

    public IVisualBuilder AddQuestionSlide(string speechText, string questionText, 
        IEnumerable<string> choices, string? footerText = null)
    {
        _layouts[LayoutNames.MultipleChoiceItem] = Layouts.MultipleChoiceItemLayout;
        _layouts[LayoutNames.MultipleChoice] = Layouts.MultipleChoiceLayout;
        
        var pageId = $"page{_components.Count + 1}";
        AddSpeech(pageId);
        
        _components.Add(new CustomComponent(LayoutNames.MultipleChoice)
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

    private void AddSpeech(string componentId)
    {
        if (_commands.Any())
        {
            _commands.Add(new SetPage
            {
                ComponentId = ComponentIds.pagerId,
                Position = SetPagePosition.Relative,
                DelayMilliseconds = 100,
                Value = 1
            });
        }
        
        _commands.Add(new SpeakItem
        {
            ComponentId = componentId
        });
    }

    public (RenderDocumentDirective?, ExecuteCommandsDirective?) GetDirectives()
    {
        return (_renderDocumentDirective, _executeCommandsDirective);
    }
}
```

## Deployment Examples

### CDK Infrastructure Stack

```csharp
public class TriviaSkillStack : Stack
{
    public TriviaSkillStack(Construct scope, string id, TriviaSkillStackProps props) : base(scope, id, props)
    {
        // DynamoDB table for game data
        var gameTable = new DynamoDbTableConstruct(this, "GameTable", new DynamoDbTableConstructProps
        {
            TableName = $"trivia-skill-{props.Environment}",
            PartitionKey = new AttributeDefinition { AttributeName = "pk", AttributeType = AttributeType.STRING },
            SortKey = new AttributeDefinition { AttributeName = "sk", AttributeType = AttributeType.STRING },
            GlobalSecondaryIndexes = 
            [
                new GlobalSecondaryIndex
                {
                    IndexName = "gs1-index",
                    PartitionKey = new AttributeDefinition { AttributeName = "gs1-pk", AttributeType = AttributeType.STRING },
                    SortKey = new AttributeDefinition { AttributeName = "gs1-sk", AttributeType = AttributeType.NUMBER },
                    ProjectionType = ProjectionType.ALL
                }
            ]
        });

        // Lambda function for Alexa skill
        var skillLambda = new LambdaFunctionConstruct(this, "SkillLambda", new LambdaFunctionConstructProps
        {
            FunctionName = "trivia-skill",
            FunctionSuffix = props.Environment,
            AssetPath = "./publish/trivia-skill.zip",
            RoleName = $"trivia-skill-{props.Environment}-role",
            PolicyName = $"trivia-skill-{props.Environment}-policy",
            MemorySize = 1024,
            TimeoutInSeconds = 30,
            PolicyStatements = 
            [
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = ["dynamodb:GetItem", "dynamodb:PutItem", "dynamodb:Query", "dynamodb:UpdateItem"],
                    Resources = [gameTable.TableArn, $"{gameTable.TableArn}/index/*"],
                    Effect = Effect.ALLOW
                })
            ],
            EnvironmentVariables = new Dictionary<string, string>
            {
                { "TABLE_NAME", gameTable.TableName },
                { "INDEX_NAME", "gs1-index" },
                { "ENVIRONMENT", props.Environment }
            }
        });

        // Alexa skill permissions
        skillLambda.LambdaFunction.AddPermission("AlexaSkillPermission", new Permission
        {
            Principal = "alexa-appkit.amazon.com",
            Action = "lambda:InvokeFunction",
            EventSourceToken = props.SkillId
        });

        // Outputs
        new CfnOutput(this, "LambdaFunctionArn", new CfnOutputProps
        {
            Value = skillLambda.LambdaFunction.FunctionArn,
            Description = "ARN of the Trivia Skill Lambda function"
        });

        new CfnOutput(this, "DynamoDbTableName", new CfnOutputProps
        {
            Value = gameTable.TableName,
            Description = "Name of the DynamoDB table"
        });
    }
}
```

### GitHub Actions Deployment

```yaml
name: Deploy Trivia Skill

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0'
    
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Test
      run: dotnet run --project test/TriviaSkill.Tests

  deploy:
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0'
        
    - name: Install AWS Lambda Tools
      run: dotnet tool install -g Amazon.Lambda.Tools
      
    - name: Build and Package
      run: |
        cd src/TriviaSkill
        dotnet lambda package
        
    - name: Deploy to AWS
      env:
        AWS_ACCESS_KEY_ID: ${{ secrets.AWS_ACCESS_KEY_ID }}
        AWS_SECRET_ACCESS_KEY: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
        AWS_REGION: us-east-1
      run: |
        cd src/TriviaSkill
        dotnet lambda deploy-function TriviaSkillFunction \
          --region us-east-1 \
          --function-role arn:aws:iam::${{ secrets.AWS_ACCOUNT_ID }}:role/trivia-skill-prod-role
```

## Common Patterns

### Environment-Specific Configuration

```csharp
public class EnvironmentConfig
{
    public string Environment { get; set; } = "dev";
    public string TableName { get; set; } = "trivia-skill-dev";
    public string IndexName { get; set; } = "gs1-index";
    public bool EnableDetailedLogging { get; set; } = false;
    public bool EnableAPL { get; set; } = true;
}

// Usage in startup
services.Configure<DynamoDbOptions>(opt =>
{
    var environment = context.Configuration.GetValue<string>("Environment") ?? "dev";
    opt.TableMaps["GameRepository"] = new DynamoDbTableMap
    {
        TableName = $"trivia-skill-{environment}",
        IndexName = "gs1-index"
    };
});
```

### Resource Naming Conventions

```csharp
public static class ResourceNaming
{
    public static string TableName(string environment) => $"trivia-skill-{environment}";
    public static string FunctionName(string environment) => $"trivia-skill-{environment}";
    public static string RoleName(string environment) => $"trivia-skill-{environment}-role";
    public static string LogGroup(string environment) => $"/aws/lambda/trivia-skill-{environment}";
}
```

### Error Recovery Patterns

```csharp
public class ResilientGameService : IGameService
{
    public async Task<Question> GetQuestionWithRetry(int bucket, Guid id, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        var delay = TimeSpan.FromMilliseconds(100);
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await _gameRepository.GetQuestionAsync(bucket, id, cancellationToken);
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                _logger.LogWarning(ex, "Attempt {attempt} failed to get question {bucket}:{id}", 
                    attempt, bucket, id);
                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // Exponential backoff
            }
        }
        
        throw new InvalidOperationException($"Failed to retrieve question after {maxRetries} attempts");
    }
}
```

## Performance Optimization Examples

### Lambda Cold Start Optimization

```csharp
// Static initialization for frequently used resources
public class OptimizedGameService
{
    private static readonly HttpClient HttpClient = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    // Pre-compile regex patterns
    private static readonly Regex AnswerPattern = new(@"^\d+$", RegexOptions.Compiled);
    
    // Cache expensive computations
    private static readonly ConcurrentDictionary<int, Question[]> QuestionCache = new();
}
```

For complete implementation details, see the trivia skill source code in the repository.