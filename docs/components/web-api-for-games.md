# Web API for Games

`AlexaVoxCraft.Model.WebApiGames` adds the skill-side models for the Alexa
[Web API for Games](https://developer.amazon.com/en-US/docs/alexa/web-api-for-games/alexa-games-about.html)
(`Alexa.Presentation.HTML` interface): the `Start` and `HandleMessage` directives, the inbound `Message`
request, speech transformers and a device capability check. It plugs into the same source-generated
`AlexaJsonOptions` pipeline and `IRequestHandler<T>` pattern as every other request type.

The package contains models and serialization only. It does not include a JavaScript bridge, a browser
SDK, game logic or hosting; your web app uses Amazon's Alexa JavaScript API, and your skill exchanges
JSON messages with it through the types below.

## :rocket: Features

- **:arrow_forward: Start**: Launch your HTTPS web app on the device with `StartDirective`
- **:outbox_tray: HandleMessage**: Send a JSON message to the running web app with `HandleMessageDirective`
- **:inbox_tray: Message**: Receive messages sent by the web app as a typed `MessageRequest`
- **:speaking_head: Transformers**: Convert text or SSML in a payload to speech with `HTMLTransformer`
- **:mag: Capability check**: Detect HTML support with `HTMLSupported()`
- **:zap: Native AOT**: Source-generated metadata, typed payloads through caller-supplied `JsonTypeInfo<T>`

## Installation

```bash
dotnet add package AlexaVoxCraft.Model.WebApiGames
```

The package depends only on `AlexaVoxCraft.Model` and targets .NET 10.0 and 11.0.

## Registration

Call `WebApiGamesSupport.Add()` once at startup, **before** any request is deserialized or a host or
serializer is built, exactly like `APLSupport.Add()` and `InSkillPurchasingSupport.Add()`:

```csharp
// Program.cs
WebApiGamesSupport.Add();
APLSupport.Add(); // optional, for an APL fallback

var builder = LambdaApplication.CreateBuilder();
builder.Services.AddSkillMediator(builder.Configuration);
builder.Services.AddAlexaSkillHost<LambdaHandler, SkillRequest, SkillResponse>();

await using var app = builder.Build();
app.MapHandler(AlexaHandler.Invoke<SkillRequest, SkillResponse>);
await app.RunAsync();
```

`WebApiGamesSupport.Add()` registers the `Alexa.Presentation.HTML.Message` request type, the `Start` and
`HandleMessage` directive types and the package's serialization metadata. It is idempotent, and its order
relative to the other packages' `Add()` calls does not matter. It is required: without it a `Message`
request fails to deserialize (unknown request types throw), and under Native AOT the directive types
have no serialization metadata.

## Capability Detection

Only start the web app on devices that advertise the HTML interface, and keep a voice (and, where
useful, APL) experience for everything else:

```csharp
public sealed class LaunchHandler : IRequestHandler<LaunchRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is LaunchRequest);

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        if (input.RequestEnvelope.HTMLSupported())
        {
            input.ResponseBuilder.AddDirective(new StartDirective(
                new StartRequest(new Uri("https://games.example.com/index.html")),
                new StartConfiguration(timeoutInSeconds: 120)));

            return await input.ResponseBuilder
                .Speak("Starting the game.")
                .GetResponse(cancellationToken);
        }

        // Voice-only (or APL) fallback.
        return await input.ResponseBuilder
            .Speak("Welcome! Say start to begin.")
            .Reprompt("Say start to begin.")
            .GetResponse(cancellationToken);
    }
}
```

`HTMLSupported()` is null-safe: it returns `false` when the request has no context, device or
supported-interfaces map. It only checks that the `Alexa.Presentation.HTML` key
(`HTMLInterface.InterfaceName`) is present. It can be combined with `APLSupported()` from
`AlexaVoxCraft.Model.Apl`.

## Starting the Web App

`StartDirective` serializes to `Alexa.Presentation.HTML.Start`:

| Type | Wire field | Rules |
|------|-----------|-------|
| `StartRequest(Uri uri, headers?)` | `request.uri`, `request.method`, `request.headers` | `uri` must be an absolute `https` URI. `method` is always `"GET"`. `headers` is omitted when not supplied. No headers are added implicitly. |
| `StartConfiguration(int timeoutInSeconds = 30)` | `configuration.timeoutInSeconds` | Seconds the web app can run without user interaction. Numeric; defaults to 30; values above 300 throw `ArgumentOutOfRangeException`. Always written. |
| `JsonElement? data` | `data` | Optional; must be a JSON object; omitted when absent. |
| `IEnumerable<HTMLTransformer>? transformers` | `transformers` | Optional; omitted when absent or empty. |

Invalid values throw when the object is constructed, so an invalid directive is never sent.

### Typed data

Pass your own source-generated metadata to serialize a typed payload without reflection:

```csharp
public sealed record GameStart(string Mode, int Level);

[JsonSerializable(typeof(GameStart))]
[JsonSerializable(typeof(GameEvent))]
internal partial class GameJsonContext : JsonSerializerContext;

var start = StartDirective.Create(
    new StartRequest(new Uri("https://games.example.com/index.html")),
    new GameStart("arcade", 1),
    GameJsonContext.Default.GameStart,
    new StartConfiguration(timeoutInSeconds: 120),
    [HTMLTransformer.SsmlToSpeech("intro", "introSpeech")]);
```

The resulting JSON:

```json
{
  "type": "Alexa.Presentation.HTML.Start",
  "request": { "uri": "https://games.example.com/index.html", "method": "GET" },
  "configuration": { "timeoutInSeconds": 120 },
  "data": { "Mode": "arcade", "Level": 1 },
  "transformers": [
    { "transformer": "ssmlToSpeech", "inputPath": "intro", "outputName": "introSpeech" }
  ]
}
```

Property names in `data` and `message` come from your context's options (here the default, so `Mode` and
`Level`); configure a naming policy on your `JsonSerializerContext` if your web app expects camelCase.

## Messages from the Web App

When the web app calls `alexa.skill.sendMessage(...)`, the skill receives an
`Alexa.Presentation.HTML.Message` request, deserialized as `MessageRequest`. Handle it like any other
request type:

```csharp
public sealed record GameEvent(string Action, int Score);

public sealed class GameMessageHandler : IRequestHandler<MessageRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is MessageRequest);

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = (MessageRequest)input.RequestEnvelope.Request;

        if (request.Message.ValueKind != JsonValueKind.Object)
        {
            return await input.ResponseBuilder.GetResponse(cancellationToken);
        }

        var gameEvent = request.GetMessage(GameJsonContext.Default.GameEvent);

        input.ResponseBuilder.AddDirective(HandleMessageDirective.Create(
            new GameEvent("ack", gameEvent?.Score ?? 0),
            GameJsonContext.Default.GameEvent));

        return await input.ResponseBuilder.GetResponse(cancellationToken);
    }
}
```

`MessageRequest.Message` is a `JsonElement` that keeps the inbound value exactly as received: any JSON
kind (object, array, string, number, boolean or null), nesting and Unicode are preserved, and a missing
`message` is `JsonValueKind.Undefined`. Nothing is deserialized into a CLR type unless you call
`GetMessage<T>` with your own `JsonTypeInfo<T>`. `GetMessage<T>` throws `InvalidOperationException` when
there is no message and `JsonException` when the payload does not match `T`. Treat the message as
untrusted input and validate it before acting on it.

## Sending Messages to the Web App

`HandleMessageDirective` serializes to `Alexa.Presentation.HTML.HandleMessage`. Its `message` is required
and must be a JSON object (anything else throws `ArgumentException` at construction). Build it from a
`JsonElement`, or from a typed value with `HandleMessageDirective.Create(value, typeInfo)` as shown above.

## Transformers

A transformer converts the text or SSML found at `inputPath` (a property of the directive's `data` or
`message` object) into speech that the web app can play:

```csharp
HTMLTransformer.TextToSpeech("prompt");                 // replaces "prompt" with the speech output
HTMLTransformer.SsmlToSpeech("intro", "introSpeech");   // writes the output to "introSpeech"
```

The factories produce the exact transformer names `textToSpeech` and `ssmlToSpeech`. `inputPath` is
required; when `outputName` is omitted, the output replaces the value at `inputPath`.

## Payload Ownership

All payloads are `JsonElement` values cloned on construction (and on assignment of
`MessageRequest.Message`), so they stay valid after the `JsonDocument` they came from is disposed.

## Native AOT

The package is `IsAotCompatible` and registers its own source-generated metadata through
`WebApiGamesSupport.Add()`, so the Web API for Games types serialize with reflection disabled. Supply
metadata for your own payload types through the `JsonTypeInfo<T>` overloads (`StartDirective.Create`,
`HandleMessageDirective.Create`, `MessageRequest.GetMessage`); there are no reflection-based `object`
overloads. See [Native AOT support](native-aot.md).

## Platform Limits

- **Size**: Amazon limits the `Start` `data` and the `HandleMessage`/`Message` `message` payloads to
  24 KB each. The package does not enforce or truncate this; keep payloads small.
- **Rate**: the web app can send at most a limited number of messages per second to the skill (Amazon's
  JavaScript API reports `too-many-requests` and rate-limit details); throttle on the client side. The
  package adds no throttling.
- **HTTPS**: the `Start` URI must be HTTPS with a valid certificate.
- **Devices**: HTML support varies by device. Always check `HTMLSupported()`, keep a voice (and
  optionally APL) fallback, and validate the experience on physical devices before relying on it.
- **Privacy**: the package never logs headers or payloads; do not log them yourself if they can contain
  personal data or tokens.
