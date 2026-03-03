# Session Management

AlexaVoxCraft provides a unified attribute management system for session, request, and persistent state in Alexa skills through `IAttributesManager` and the `JsonAttributeBag` type.

## Features

- **Session Attributes**: Automatic hydration from the incoming request's session
- **Request Attributes**: Per-request in-memory state scoped to the current invocation
- **Persistent Attributes**: Lazily-loaded, adapter-backed storage (e.g. DynamoDB)
- **Typed Access**: `Get<T>`, `Set<T>`, `TryGet<T>`, and `GetRequired<T>` on all attribute bags
- **JsonElement-Based**: All values stored as `JsonElement`, serialized via `System.Text.Json`

## IAttributesManager

`IAttributesManager` is injected into handlers and interceptors via `IHandlerInput`:

```csharp
public interface IAttributesManager
{
    JsonAttributeBag Session { get; }
    JsonAttributeBag Request { get; }
    Task<JsonAttributeBag> GetPersistentAsync(CancellationToken ct = default);
    Task SavePersistentAttributes(CancellationToken cancellationToken = default);
    Task<Session?> GetSession(CancellationToken cancellationToken = default);
}
```

## JsonAttributeBag

`JsonAttributeBag` wraps `Dictionary<string, JsonElement>` with typed, serialization-aware access methods. All serialization uses `AlexaJsonOptions.DefaultOptions`.

```csharp
// Typed write
bag.Set<int>("score", 42);
bag.Set<Product[]>("entitledProducts", products);

// Typed read (returns default if key missing)
var score = bag.Get<int>("score");

// Try pattern
if (bag.TryGet<Product[]>("entitledProducts", out var products))
{
    // use products
}

// Required — throws KeyNotFoundException if missing
var state = bag.GetRequired<GameState>("gameState");

// Removal
bag.Remove("tempKey");
bag.Clear();

// Raw dictionary access (e.g. for assigning to SkillResponse.SessionAttributes)
Dictionary<string, JsonElement> raw = bag.Values;
```

## Session Attributes

`IAttributesManager.Session` is a `JsonAttributeBag` initialized from the incoming request's `Session.Attributes`. It is synchronous and always available (empty bag if the session has no attributes).

```csharp
public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
{
    // Read
    var score = input.AttributesManager.Session.Get<int>("currentScore");

    // Write
    input.AttributesManager.Session.Set("currentScore", score + 1);

    return await input.ResponseBuilder
        .Speak($"Your score is now {score + 1}.")
        .GetResponse(cancellationToken);
}
```

The `DefaultResponseBuilder` automatically copies `Session.Values` into `SkillResponse.SessionAttributes` when building the response — no manual step required.

## Request Attributes

`IAttributesManager.Request` is an empty `JsonAttributeBag` scoped to the current invocation. Use it to share computed data between interceptors and handlers without round-tripping through session.

```csharp
// In a request interceptor
public async Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
{
    var products = await _productService.GetEntitledProductsAsync(cancellationToken);
    input.AttributesManager.Request.Set("entitledProducts", products);
}

// In a handler
public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
{
    var products = input.AttributesManager.Request.Get<Product[]>("entitledProducts");
    // ...
}
```

## Persistent Attributes

Persistent attributes require an `IPersistenceAdapter` implementation registered in DI. They are loaded lazily on first access and must be explicitly saved.

### IPersistenceAdapter

```csharp
public interface IPersistenceAdapter
{
    Task<IDictionary<string, JsonElement>> GetAttributes(SkillRequest requestEnvelope,
        CancellationToken cancellationToken = default);

    Task SaveAttribute(SkillRequest requestEnvelope, IDictionary<string, JsonElement> attributes,
        CancellationToken cancellationToken = default);
}
```

### Example Implementation

```csharp
public sealed class DynamoDbPersistenceAdapter : IPersistenceAdapter
{
    private readonly IAmazonDynamoDB _client;
    private readonly string _tableName;

    public DynamoDbPersistenceAdapter(IAmazonDynamoDB client, string tableName)
    {
        _client = client;
        _tableName = tableName;
    }

    public async Task<IDictionary<string, JsonElement>> GetAttributes(
        SkillRequest requestEnvelope, CancellationToken cancellationToken = default)
    {
        var userId = requestEnvelope.Session?.User?.UserId
            ?? throw new InvalidOperationException("No user ID in request.");

        var response = await _client.GetItemAsync(new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "userId", new AttributeValue { S = userId } }
            }
        }, cancellationToken);

        if (response.Item is null || response.Item.Count == 0)
            return new Dictionary<string, JsonElement>();

        // Deserialize stored JSON blob back to JsonElement dictionary
        var json = response.Item["attributes"].S;
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
            ?? new Dictionary<string, JsonElement>();
    }

    public async Task SaveAttribute(SkillRequest requestEnvelope,
        IDictionary<string, JsonElement> attributes, CancellationToken cancellationToken = default)
    {
        var userId = requestEnvelope.Session?.User?.UserId
            ?? throw new InvalidOperationException("No user ID in request.");

        var json = JsonSerializer.Serialize(attributes);

        await _client.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "userId", new AttributeValue { S = userId } },
                { "attributes", new AttributeValue { S = json } }
            }
        }, cancellationToken);
    }
}
```

### Registration

```csharp
services.AddSkillMediator(configuration, cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

services.AddSingleton<IPersistenceAdapter, DynamoDbPersistenceAdapter>();
```

### Reading and Saving

```csharp
public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
{
    var persistent = await input.AttributesManager.GetPersistentAsync(cancellationToken);

    var totalGames = persistent.Get<int>("totalGames");
    persistent.Set("totalGames", totalGames + 1);

    await input.AttributesManager.SavePersistentAttributes(cancellationToken);

    return await input.ResponseBuilder
        .Speak($"You have played {totalGames + 1} games total.")
        .GetResponse(cancellationToken);
}
```

`SavePersistentAttributes` is a no-op if `GetPersistentAsync` was never called, preventing unnecessary writes.

## Interceptor Pattern

Interceptors are the recommended place to load and save state, keeping handlers focused on response logic.

### Request Interceptor

```csharp
public sealed class LoadStateInterceptor : IRequestInterceptor
{
    public async Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var persistent = await input.AttributesManager.GetPersistentAsync(cancellationToken);
        input.AttributesManager.Request.Set("playerProfile", persistent.Get<PlayerProfile>("profile"));
    }
}
```

### Response Interceptor

```csharp
public sealed class SaveStateInterceptor : IResponseInterceptor
{
    public async Task Process(IHandlerInput input, SkillResponse response,
        CancellationToken cancellationToken = default)
    {
        var profile = input.AttributesManager.Request.Get<PlayerProfile>("playerProfile");
        var persistent = await input.AttributesManager.GetPersistentAsync(cancellationToken);
        persistent.Set("profile", profile);
        await input.AttributesManager.SavePersistentAttributes(cancellationToken);
    }
}
```

## Checking Session State in CanHandle

```csharp
public Task<bool> CanHandle(IHandlerInput handlerInput, CancellationToken cancellationToken = default)
{
    var isNewSession = handlerInput.RequestEnvelope.Session?.New == true;
    var hasGameStarted = handlerInput.AttributesManager.Session.TryGet<bool>("gameStarted", out var started)
        && started;

    return Task.FromResult(isNewSession || !hasGameStarted);
}
```

## Raw Session Object

Use `GetSession` when you need the full `Session` model (e.g. user ID, application ID):

```csharp
var session = await input.AttributesManager.GetSession(cancellationToken);
var userId = session?.User?.UserId;
```