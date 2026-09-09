using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Http.Clients;
using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda.Serialization;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;
using AlexaVoxCraft.NativeAot.ValidationApp;
using AlexaVoxCraft.Smapi.Builders.InteractionModel;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.Invocation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

var failures = new List<string>();
void Check(string name, bool ok)
{
    Console.WriteLine(ok ? $"PASS: {name}" : $"FAIL: {name}");
    if (!ok) failures.Add(name);
}

void CheckException<TException>(string name, Action act) where TException : Exception
{
    try
    {
        act();
        Console.WriteLine($"FAIL: {name} (expected {typeof(TException).Name}, none thrown)");
        failures.Add(name);
    }
    catch (TException)
    {
        Console.WriteLine($"PASS: {name}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FAIL: {name} (expected {typeof(TException).Name}, got {ex.GetType().Name}: {ex.Message})");
        failures.Add(name);
    }
}

// ---------------------------------------------------------------------------
// Scenario 1: Core Alexa JSON - deserialize a real LaunchRequest, serialize a response
// with a card and a directive (exercises the polymorphic converters).
// ---------------------------------------------------------------------------
{
    const string launchJson = """
    {
      "version": "1.0",
      "session": { "new": true, "sessionId": "s1", "application": { "applicationId": "amzn1.ask.skill.validation" }, "user": { "userId": "u1" } },
      "context": {
        "System": { "application": { "applicationId": "amzn1.ask.skill.validation" }, "user": { "userId": "u1" }, "device": { "deviceId": "d1", "supportedInterfaces": {} }, "apiEndpoint": "https://api.amazonalexa.com", "apiAccessToken": "token" }
      },
      "request": { "type": "LaunchRequest", "requestId": "r1", "timestamp": "2026-01-01T00:00:00Z", "locale": "en-US" }
    }
    """;

    var request = JsonSerializer.Deserialize<SkillRequest>(launchJson, AlexaJsonOptions.DefaultOptions);
    Check("Deserialize LaunchRequest", request?.Request is LaunchRequest);

    var response = new SkillResponse
    {
        Version = "1.0",
        Response = new ResponseBody
        {
            OutputSpeech = new PlainTextOutputSpeech { Text = "hello" },
            Card = new SimpleCard { Title = "t", Content = "c" },
            ShouldEndSession = true
        }
    };
    var json = JsonSerializer.Serialize(response, AlexaJsonOptions.DefaultOptions);
    Check("Serialize SkillResponse with card", json.Contains("\"card\""));
}

// ---------------------------------------------------------------------------
// Scenario 2: APL - APLSupport.Add(), component graph round trip.
// ---------------------------------------------------------------------------
{
    APLSupport.Add();

    var document = new APLDocument
    {
        MainTemplate = new Layout(new Container
        {
            Items = [new Text("hello apl")]
        })
    };

    var directive = new RenderDocumentDirective(document);
    var json = JsonSerializer.Serialize<IDirective>(directive, AlexaJsonOptions.DefaultOptions);
    Check("Serialize APL RenderDocumentDirective", json.Contains("Container") && json.Contains("hello apl"));

    var roundTripped = JsonSerializer.Deserialize<IDirective>(json, AlexaJsonOptions.DefaultOptions);
    Check("Deserialize APL RenderDocumentDirective", roundTripped is RenderDocumentDirective);
}

// ---------------------------------------------------------------------------
// Scenario 3: Consumer-owned metadata - a validation-app-owned POCO with its own
// generated context, registered via RegisterTypeInfoResolver, used through JsonAttributeBag.
// Also proves library metadata still wins for library types after the consumer context registers.
// ---------------------------------------------------------------------------
{
    AlexaJsonOptions.RegisterTypeInfoResolver(ValidationAppContext.Default);

    var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
    bag.Set("state", new GameState { Score = 42, Level = "final" });
    var readBack = bag.Get<GameState>("state");
    Check("Consumer POCO round-trips via JsonAttributeBag", readBack is { Score: 42, Level: "final" });

    var stillWorks = JsonSerializer.Deserialize<LaunchRequest>("""{"type":"LaunchRequest"}""", AlexaJsonOptions.DefaultOptions);
    Check("Library type still resolves after consumer registration", stillWorks is not null);
}

// ---------------------------------------------------------------------------
// Scenario 4: Missing consumer metadata fails clearly and immediately (required negative scenario).
// ---------------------------------------------------------------------------
{
    CheckException<NotSupportedException>("Unregistered type fails clearly under reflection-disabled AOT",
        () => JsonSerializer.Serialize(new UnregisteredValidationPoco { Value = "x" }, AlexaJsonOptions.DefaultOptions));
}

// ---------------------------------------------------------------------------
// Scenario 5: Mediator - SkillMediator.Send through the generator-produced keyed dispatch path.
// ---------------------------------------------------------------------------
{
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?> { ["Skill:SkillId"] = "amzn1.ask.skill.validation" })
        .Build();

    var services = new ServiceCollection();
    services.AddSkillMediator(configuration, cfg =>
    {
        cfg.SkillId = "amzn1.ask.skill.validation";
        cfg.RegisterServicesFromAssemblyContaining<LaunchHandler>();
    });

    services.AddScoped<SkillRequestFactory>(sp => () => AmbientRequest.Current);
    services.AddLogging(b => b.AddConsole());

    var provider = services.BuildServiceProvider();

    var skillRequest = new SkillRequest
    {
        Request = new LaunchRequest { Type = "LaunchRequest" },
        Context = new Context
        {
            System = new AlexaSystem { Application = new Application { ApplicationId = "amzn1.ask.skill.validation" } }
        }
    };
    AmbientRequest.Current = skillRequest;

    using var scope = provider.CreateScope();
    var mediator = scope.ServiceProvider.GetRequiredService<ISkillMediator>();
    var response = mediator.Send(skillRequest, CancellationToken.None).GetAwaiter().GetResult();

    Check("SkillMediator.Send dispatches via generated handler",
        response.Response?.OutputSpeech is SsmlOutputSpeech ssml
        && ssml.Ssml.Contains("hello from the native AOT validation app"));
}

// ---------------------------------------------------------------------------
// Scenario 6: ISP/Http against a validation-app-local fake HttpMessageHandler (no Compono.Http).
// ---------------------------------------------------------------------------
{
    var handler = new FakeHttpMessageHandler((_, _) =>
    {
        const string body = """{"inSkillProducts":[],"isTruncated":false}""";
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };
    });
    var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazonalexa.com/") };
    var ispClient = new InSkillPurchasingClient(httpClient, NullLogger<InSkillPurchasingClient>.Instance);

    var products = ispClient.GetProductsAsync(cancellationToken: CancellationToken.None).GetAwaiter().GetResult();
    Check("InSkillPurchasingClient.GetProductsAsync (AOT-safe default resolver)", products is not null);
}

// ---------------------------------------------------------------------------
// Scenario 7: SMAPI - standalone InteractionModelBuilder.ToJson() (no DI), and
// AlexaSkillInvocationClient.InvokeAsync with a consumer-owned type, against a fake transport.
// ---------------------------------------------------------------------------
{
    var builder = InteractionModelBuilder.Create()
        .WithInvocationName("validation skill")
        .WithVersion("1.0")
        .WithDescription("native aot validation app");
    var json = builder.ToJson();
    Check("InteractionModelBuilder.ToJson() works standalone, no DI container", !string.IsNullOrWhiteSpace(json));

    var invocationHandler = new FakeHttpMessageHandler((_, _) =>
    {
        var responseBody = JsonSerializer.Serialize(new SkillInvocationResponse<GameState>
        {
            Status = "SUCCESSFUL",
            Result = new SkillInvocationResult<GameState>
            {
                SkillExecutionInfo = new SkillExecutionInfo<GameState>
                {
                    InvocationResponse = new InvocationResponseInfo<GameState> { Body = new GameState { Score = 7, Level = "smapi" } }
                }
            }
        }, AlexaJsonOptions.DefaultOptions);
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseBody) };
    });
    var invocationClient = new AlexaSkillInvocationClient(
        new HttpClient(invocationHandler) { BaseAddress = new Uri("https://api.amazonalexa.com/") },
        NullLogger<AlexaSkillInvocationClient>.Instance);

    var invocationResult = invocationClient.InvokeAsync<GameState, GameState>(
        "skill-id", "development", new GameState { Score = 1, Level = "start" }, ct: CancellationToken.None).GetAwaiter().GetResult();

    Check("AlexaSkillInvocationClient.InvokeAsync with consumer-owned type",
        invocationResult?.Result?.SkillExecutionInfo?.InvocationResponse?.Body is { Score: 7, Level: "smapi" });
}

// ---------------------------------------------------------------------------
// Scenario 8: Lambda boundary - the real AlexaLambdaSerializer, not just JSON calls in isolation.
// ---------------------------------------------------------------------------
{
    var serializer = new AlexaLambdaSerializer(NullLogger<AlexaLambdaSerializer>.Instance, AlexaJsonOptions.DefaultOptions);
    var payload = new SkillRequest
    {
        Request = new LaunchRequest { Type = "LaunchRequest" },
        Context = new Context { System = new AlexaSystem { Application = new Application { ApplicationId = "a" } } }
    };

    using var stream = new MemoryStream();
    serializer.Serialize(payload, stream);
    stream.Position = 0;
    var roundTripped = serializer.Deserialize<SkillRequest>(stream);

    Check("AlexaLambdaSerializer round-trips SkillRequest", roundTripped?.Request is LaunchRequest);
}

Console.WriteLine();
Console.WriteLine(failures.Count == 0
    ? $"ALL SCENARIOS PASSED"
    : $"{failures.Count} SCENARIO(S) FAILED: {string.Join(", ", failures)}");

return failures.Count == 0 ? 0 : 1;

file static class AmbientRequest
{
    [ThreadStatic]
    private static SkillRequest? _current;
    public static SkillRequest? Current { get => _current; set => _current = value; }
}

file sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> responder) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(responder(request, cancellationToken));
}

public sealed class GameState
{
    public int Score { get; set; }
    public string? Level { get; set; }
}

public sealed class UnregisteredValidationPoco
{
    public string? Value { get; set; }
}

[JsonSerializable(typeof(GameState))]
[JsonSerializable(typeof(SkillInvocationResponse<GameState>))]
[JsonSerializable(typeof(SkillInvocationRequest<GameState>))]
[JsonSerializable(typeof(SkillInvocationBody<GameState>))]
[JsonSerializable(typeof(SkillInvocationResult<GameState>))]
[JsonSerializable(typeof(SkillExecutionInfo<GameState>))]
[JsonSerializable(typeof(InvocationResponseInfo<GameState>))]
internal partial class ValidationAppContext : JsonSerializerContext;
