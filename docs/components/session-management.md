# Session Management

AlexaVoxCraft provides robust session and state management capabilities for building multi-turn conversations and persistent user experiences in Alexa skills.

> 🎯 **Trivia Skill Examples**: All code examples demonstrate managing **trivia game state** including current question, score tracking, player data, and multi-question game sessions.

## :rocket: Features

- **:floppy_disk: Session Persistence**: Automatic session attribute management
- **:video_game: Game State Tracking**: Complex game state with multiple properties
- **:busts_in_silhouette: User Data Management**: Persistent user profiles and progress
- **:arrows_counterclockwise: Multi-Turn Conversations**: Stateful conversation flows
- **:file_cabinet: DynamoDB Integration**: Scalable data persistence patterns
- **:gear: Attribute Helpers**: Type-safe attribute access and manipulation

## Basic Usage

### Session Attributes

```csharp
public class LaunchRequestHandler : IRequestHandler<LaunchRequest>
{
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        // Get session attributes
        var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
        
        // Set session data
        sessionAttributes["gameStarted"] = true;
        sessionAttributes["currentScore"] = 0;
        sessionAttributes["questionIndex"] = 0;
        
        // Save back to session
        await input.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);
        
        return await input.ResponseBuilder
            .Speak("Welcome! Let's start the game.")
            .GetResponse(cancellationToken);
    }
}
```

### Persistent Attributes

```csharp
public class GameService : IGameService
{
    public async Task LoadCurrentPlayer(string userId, CancellationToken cancellationToken)
    {
        // Load from persistent storage (DynamoDB)
        var player = await _gameRepository.GetPlayerAsync(userId, cancellationToken);
        
        if (player == null)
        {
            // Create new player
            player = new Player
            {
                UserId = userId,
                Username = $"Player{userId.GetHashCode()}",
                Score = 0,
                TotalGames = 0,
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            await _gameRepository.SavePlayerAsync(player, cancellationToken);
        }
        
        CurrentPlayer = player;
    }
}
```

## Game State Management

### Game State Model

```csharp
public class GameState
{
    public Dictionary<int, Guid> GameQuestions { get; set; } = new();
    public int CorrectAnswerIndex { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int CurrentScore { get; set; }
    public string CorrectAnswerText { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public IEnumerable<string> Answers { get; set; } = [];
}
```

### Game Manager Service

From the trivia skill implementation:

```csharp
public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<GameManagerService> _logger;
    private readonly Random _random;

    public Player? CurrentPlayer { get; private set; }
    public GameState CurrentGame { get; private set; }

    public GameService(ILogger<GameService> logger, IGameRepository gameRepository)
    {
        _logger = logger;
        _gameRepository = gameRepository;
        _random = Random.Shared;
        CurrentGame = new GameState();
    }

    public Task LoadCurrentGame(IDictionary<string, object> attributes, CancellationToken cancellationToken)
    {
        try
        {
            // Load game state from session attributes
            CurrentGame.GameQuestions = attributes.TryParseIntDictionary(Attributes.Questions);
            CurrentGame.CorrectAnswerIndex = attributes.TryParseInt(Attributes.CorrectAnswerIndex);
            CurrentGame.CurrentScore = attributes.TryParseInt(Attributes.Score);
            CurrentGame.CurrentQuestionIndex = attributes.TryParseInt(Attributes.CurrentQuestionIndex);
            CurrentGame.CorrectAnswerText = attributes.TryParseString(Attributes.CorrectAnswerText);
            CurrentGame.QuestionText = attributes.TryParseString(Attributes.QuestionText);
            CurrentGame.Answers = attributes.TryParseStringList(Attributes.Answers);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to load current game state: {message}", exception.Message);
        }

        return Task.CompletedTask;
    }

    public Task SaveCurrentGame(IDictionary<string, object> attributes, CancellationToken cancellationToken)
    {
        // Save game state to session attributes
        attributes[Attributes.Questions] = CurrentGame.GameQuestions;
        attributes[Attributes.CorrectAnswerIndex] = CurrentGame.CorrectAnswerIndex;
        attributes[Attributes.CurrentQuestionIndex] = CurrentGame.CurrentQuestionIndex;
        attributes[Attributes.Score] = CurrentGame.CurrentScore;
        attributes[Attributes.CorrectAnswerText] = CurrentGame.CorrectAnswerText;
        attributes[Attributes.QuestionText] = CurrentGame.QuestionText;
        attributes[Attributes.Answers] = CurrentGame.Answers;

        return Task.FromResult(attributes);
    }

    public async Task StartNewGame(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting a new game");
        
        // Generate random questions
        var gameQuestions = await PopulateGameQuestions();
        var correctAnswerIndex = _random.Next(0, 4);
        var (bucket, id) = gameQuestions.First();
        var question = await GetQuestion(bucket, id, cancellationToken);
        var roundAnswers = PopulateRoundAnswers(question, correctAnswerIndex);

        CurrentGame.GameQuestions = gameQuestions;
        CurrentGame.CorrectAnswerIndex = correctAnswerIndex;
        CurrentGame.QuestionText = question.Message;
        CurrentGame.Answers = roundAnswers.Select(answer => answer.Message);
        CurrentGame.CurrentQuestionIndex = 0;
        CurrentGame.CurrentScore = 0;
        CurrentGame.CorrectAnswerText = question.Answers[0].Message;

        _logger.LogDebug("New game started: {@gameState}", CurrentGame);
    }

    public async Task LoadNextQuestion(CancellationToken cancellationToken)
    {
        var currentQuestionIndex = CurrentGame.CurrentQuestionIndex + 1;
        var correctAnswerIndex = _random.Next(0, 4);
        var (bucket, id) = CurrentGame.GameQuestions.ElementAt(currentQuestionIndex);
        
        var question = await GetQuestion(bucket, id, cancellationToken);
        var roundAnswers = PopulateRoundAnswers(question, correctAnswerIndex);

        CurrentGame.CorrectAnswerIndex = correctAnswerIndex;
        CurrentGame.CurrentQuestionIndex = currentQuestionIndex;
        CurrentGame.CorrectAnswerText = question.Answers[0].Message;
        CurrentGame.QuestionText = question.Message;
        CurrentGame.Answers = roundAnswers.Select(answer => answer.Message);
    }

    public Task<bool> UpdateScore(int submittedAnswer, CancellationToken cancellationToken)
    {
        var answerCorrect = submittedAnswer - 1 == CurrentGame.CorrectAnswerIndex;
        
        if (CurrentPlayer != null)
        {
            CurrentPlayer.TotalAnswers++;
            if (answerCorrect)
            {
                CurrentPlayer.CorrectAnswers++;
                CurrentGame.CurrentScore++;
            }

            CurrentPlayer.PercentageCorrect = (double)CurrentPlayer.CorrectAnswers / CurrentPlayer.TotalAnswers * 100;
            CurrentPlayer.Score = (CurrentPlayer.CorrectAnswers * 100) + (int)(CurrentPlayer.PercentageCorrect * 10);
            CurrentPlayer.UpdatedDate = DateTimeOffset.UtcNow;
        }

        return Task.FromResult(answerCorrect);
    }
}
```

## Attribute Helpers

### Type-Safe Attribute Access

```csharp
public static class DictionaryExtensions
{
    public static string TryParseString(this IDictionary<string, object> attributes, string key)
    {
        return attributes.TryGetValue(key, out var value) && value is string str ? str : string.Empty;
    }

    public static int TryParseInt(this IDictionary<string, object> attributes, string key)
    {
        if (attributes.TryGetValue(key, out var value))
        {
            return value switch
            {
                int intValue => intValue,
                string strValue when int.TryParse(strValue, out var parsed) => parsed,
                _ => 0
            };
        }
        return 0;
    }

    public static Dictionary<int, Guid> TryParseIntDictionary(this IDictionary<string, object> attributes, string key)
    {
        if (attributes.TryGetValue(key, out var value) && value is Dictionary<string, object> dict)
        {
            var result = new Dictionary<int, Guid>();
            foreach (var kvp in dict)
            {
                if (int.TryParse(kvp.Key, out var intKey) && Guid.TryParse(kvp.Value?.ToString(), out var guidValue))
                {
                    result[intKey] = guidValue;
                }
            }
            return result;
        }
        return new Dictionary<int, Guid>();
    }

    public static IEnumerable<string> TryParseStringList(this IDictionary<string, object> attributes, string key)
    {
        if (attributes.TryGetValue(key, out var value))
        {
            return value switch
            {
                IEnumerable<string> stringList => stringList,
                IEnumerable<object> objectList => objectList.Select(o => o?.ToString() ?? string.Empty),
                _ => []
            };
        }
        return [];
    }
}
```

### Attribute Constants

```csharp
public static class Attributes
{
    public const string Questions = "QUESTIONS";
    public const string CorrectAnswerIndex = "CORRECT_ANSWER_INDEX";
    public const string CurrentQuestionIndex = "CURRENT_QUESTION_INDEX";
    public const string Score = "SCORE";
    public const string CorrectAnswerText = "CORRECT_ANSWER_TEXT";
    public const string QuestionText = "QUESTION_TEXT";
    public const string Answers = "ANSWERS";
    public const string SpeechOutput = "SPEECH_OUTPUT";
    public const string RepromptText = "REPROMPT_TEXT";
}
```

## User Data Persistence

### Player Model

```csharp
public class Player : BaseItem, IBaseItem<Player>
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int CorrectAnswers { get; set; }
    public int TotalAnswers { get; set; }
    public int Score { get; set; }
    public double PercentageCorrect { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public string PlayerType { get; set; } = "active";
    
    // DynamoDB GSI support
    public string Gs1Pk => "game#trivia-skill#player-type#active";
    public string Gs1Sk => Score.ToString().PadLeft(10, '0');

    public static Player CreateFromAttributes(Dictionary<string, AttributeValue> attributes)
    {
        return new Player
        {
            UserId = attributes.GetValueOrDefault("userId")?.S ?? string.Empty,
            Username = attributes.GetValueOrDefault("username")?.S ?? string.Empty,
            CorrectAnswers = int.Parse(attributes.GetValueOrDefault("correctAnswers")?.N ?? "0"),
            TotalAnswers = int.Parse(attributes.GetValueOrDefault("totalAnswers")?.N ?? "0"),
            Score = int.Parse(attributes.GetValueOrDefault("score")?.N ?? "0"),
            PercentageCorrect = double.Parse(attributes.GetValueOrDefault("percentageCorrect")?.N ?? "0"),
            CreatedDate = DateTimeOffset.Parse(attributes.GetValueOrDefault("createdDate")?.S ?? DateTimeOffset.UtcNow.ToString()),
            UpdatedDate = DateTimeOffset.Parse(attributes.GetValueOrDefault("updatedDate")?.S ?? DateTimeOffset.UtcNow.ToString()),
            PlayerType = attributes.GetValueOrDefault("playerType")?.S ?? "active"
        };
    }

    public Dictionary<string, AttributeValue> ToItem()
    {
        return new Dictionary<string, AttributeValue>
        {
            { "pk", new AttributeValue { S = $"user#{UserId}" } },
            { "sk", new AttributeValue { S = "game#trivia-skill" } },
            { "gs1-pk", new AttributeValue { S = Gs1Pk } },
            { "gs1-sk", new AttributeValue { N = Gs1Sk } },
            { "userId", new AttributeValue { S = UserId } },
            { "username", new AttributeValue { S = Username } },
            { "correctAnswers", new AttributeValue { N = CorrectAnswers.ToString() } },
            { "totalAnswers", new AttributeValue { N = TotalAnswers.ToString() } },
            { "score", new AttributeValue { N = Score.ToString() } },
            { "percentageCorrect", new AttributeValue { N = PercentageCorrect.ToString("F2") } },
            { "createdDate", new AttributeValue { S = CreatedDate.ToString("O") } },
            { "updatedDate", new AttributeValue { S = UpdatedDate.ToString("O") } },
            { "playerType", new AttributeValue { S = PlayerType } }
        };
    }
}
```

### Repository Pattern

```csharp
public interface IGameRepository
{
    Task<Player?> GetPlayerAsync(string userId, CancellationToken cancellationToken);
    Task SavePlayerAsync(Player player, CancellationToken cancellationToken);
    Task<IEnumerable<Player>> RetrieveTopPlayers(int numberOfPlayers = 5, CancellationToken cancellationToken = default);
    Task<int> RetrievePlayerRank(Player player, CancellationToken cancellationToken = default);
    Task<Question> GetQuestionAsync(int bucket, Guid id, CancellationToken cancellationToken = default);
    Task<int> GetQuestionCountAsync(CancellationToken cancellationToken = default);
}

public class GameRepository : IGameRepository
{
    private readonly IAmazonDynamoDB _client;
    private readonly ILogger<GameRepository> _logger;
    private readonly DynamoDbTableMap _options;

    public async Task<Player?> GetPlayerAsync(string userId, CancellationToken cancellationToken)
    {
        var pk = $"user#{userId}";
        const string sk = "game#trivia-skill";
        
        var request = new GetItemRequest
        {
            TableName = _options.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = pk } },
                { "sk", new AttributeValue { S = sk } }
            },
            ConsistentRead = true
        };

        var response = await _client.GetItemAsync(request, cancellationToken);
        
        return response.Item?.Count > 0 
            ? Player.CreateFromAttributes(response.Item) 
            : null;
    }

    public async Task SavePlayerAsync(Player player, CancellationToken cancellationToken)
    {
        var request = new PutItemRequest
        {
            TableName = _options.TableName,
            Item = player.ToItem()
        };

        await _client.PutItemAsync(request, cancellationToken);
        _logger.LogDebug("Saved player {userId}", player.UserId);
    }
}
```

## Request/Response Interceptors

### Request Interceptor for State Loading

```csharp
public class GameServiceRequestInterceptor : IRequestInterceptor
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameServiceRequestInterceptor> _logger;

    public GameServiceRequestInterceptor(IGameService gameService, 
        ILogger<GameServiceRequestInterceptor> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public async Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var userId = input.RequestEnvelope.GetUserId();
        _logger.LogDebug("Loading player for {userId}", userId);
        
        var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
        
        // Load both player and game state in parallel
        var loadPlayerTask = _gameService.LoadCurrentPlayer(userId, cancellationToken);
        var loadGameTask = _gameService.LoadCurrentGame(sessionAttributes, cancellationToken);

        await Task.WhenAll(loadPlayerTask, loadGameTask);
    }
}
```

### Response Interceptor for State Saving

```csharp
public class GameServiceResponseInterceptor : IResponseInterceptor
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameServiceResponseInterceptor> _logger;

    public async Task Process(IHandlerInput input, SkillResponse response, CancellationToken cancellationToken = default)
    {
        // Save game state to session
        var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
        await _gameService.SaveCurrentGame(sessionAttributes, cancellationToken);
        await input.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);

        // Save player data to persistent storage
        await _gameService.SaveCurrentPlayer(cancellationToken);
        
        _logger.LogDebug("Saved game and player state");
    }
}
```

## Multi-Turn Conversation Patterns

### Conversation State Machine

```csharp
public enum ConversationState
{
    WaitingForStart,
    InGame,
    WaitingForAnswer,
    GameOver,
    ShowingLeaderboard
}

public class ConversationManager
{
    public static ConversationState GetCurrentState(IDictionary<string, object> sessionAttributes)
    {
        var stateString = sessionAttributes.TryParseString("CONVERSATION_STATE");
        return Enum.TryParse<ConversationState>(stateString, out var state) 
            ? state 
            : ConversationState.WaitingForStart;
    }

    public static void SetState(IDictionary<string, object> sessionAttributes, ConversationState state)
    {
        sessionAttributes["CONVERSATION_STATE"] = state.ToString();
    }
}

// Usage in handlers
public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
{
    var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
    var currentState = ConversationManager.GetCurrentState(sessionAttributes);
    
    var response = currentState switch
    {
        ConversationState.WaitingForStart => await HandleStart(input, cancellationToken),
        ConversationState.InGame => await HandleGameplay(input, cancellationToken),
        ConversationState.GameOver => await HandleGameOver(input, cancellationToken),
        _ => await HandleDefault(input, cancellationToken)
    };

    ConversationManager.SetState(sessionAttributes, ConversationState.InGame);
    await input.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);
    
    return response;
}
```

## Leaderboard and Rankings

### Top Players Query

```csharp
public async Task<IEnumerable<Player>> RetrieveTopPlayers(int numberOfPlayers = 5, 
    CancellationToken cancellationToken = default)
{
    var request = new QueryRequest
    {
        TableName = _options.TableName,
        IndexName = _options.IndexName, // GSI on score
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
```

### Player Ranking

```csharp
public async Task<int> RetrievePlayerRank(Player player, CancellationToken cancellationToken = default)
{
    var request = new QueryRequest
    {
        TableName = _options.TableName,
        IndexName = _options.IndexName,
        ScanIndexForward = false,
        KeyConditionExpression = "#gs1pk = :pk AND #gs1sk > :playerScore",
        ExpressionAttributeNames = new Dictionary<string, string>
        {
            { "#gs1pk", "gs1-pk" },
            { "#gs1sk", "gs1-sk" }
        },
        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":pk", new AttributeValue { S = player.Gs1Pk } },
            { ":playerScore", new AttributeValue { N = player.Score.ToString() } }
        }
    };

    var response = await _client.QueryAsync(request, cancellationToken);
    return response.Items.Count + 1; // Rank is count of higher scores + 1
}
```

## Best Practices

### 1. Handle Session Expiration

```csharp
public Task<bool> CanHandle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
{
    // Check if session is new or if game state exists
    var isNewSession = handlerInput.RequestEnvelope.Session?.New == true;
    var hasGameState = handlerInput.RequestEnvelope.Session?.Attributes?.ContainsKey("GAME_STARTED") == true;
    
    return Task.FromResult(isNewSession || !hasGameState);
}
```

### 2. Implement Data Validation

```csharp
private void ValidateGameState(GameState gameState)
{
    if (gameState.CurrentQuestionIndex < 0 || gameState.CurrentQuestionIndex >= GameLength)
        throw new InvalidOperationException("Invalid question index");
        
    if (gameState.CurrentScore < 0 || gameState.CurrentScore > GameLength)
        throw new InvalidOperationException("Invalid score");
}
```

### 3. Use Strongly Typed Extensions

```csharp
public static class SkillRequestExtensions
{
    public static string GetUserId(this SkillRequest request)
    {
        return request.Context?.System?.User?.UserId ?? 
               request.Session?.User?.UserId ?? 
               throw new InvalidOperationException("User ID not found");
    }
    
    public static bool IsNewSession(this SkillRequest request)
    {
        return request.Session?.New == true;
    }
}
```

### 4. Log State Changes

```csharp
_logger.LogDebug("Game state changed: from {@oldState} to {@newState}", 
    oldState, 
    newState);
```

## Examples

For complete session management examples, see the [Examples](../examples/index.md) section with the trivia skill's comprehensive state management implementation.