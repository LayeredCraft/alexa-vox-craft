using System.Text.Json;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AlexaVoxCraft.MediatR.Tests.WebApiGames;

/// <summary>
/// End-to-end dispatch proof for Issue #213: an ordinary generic <see cref="IRequestHandler{TRequestType}"/>
/// for a Web API for Games <see cref="AlexaVoxCraft.Model.WebApiGames.MessageRequest"/> receives a typed,
/// deserialized request through the real <see cref="ISkillMediator"/> and the generator-backed keyed
/// dispatch path, then serializes its response directive through <see cref="AlexaJsonOptions.DefaultOptions"/>.
/// The <see cref="SkillRequest"/> always comes from real JSON deserialization after
/// <see cref="AlexaVoxCraft.Model.WebApiGames.WebApiGamesSupport.Add"/> (run once in
/// <see cref="TestModuleInitializer"/>); there is no fake model serializer and no separate mediator.
/// </summary>
public sealed class WebApiGamesDispatchTests : TestBase
{
    private const string SkillId = "amzn1.ask.skill.0123456789";

    [Fact]
    public async Task Send_MessageRequestWithHandleAction_DispatchesAndSerializesHandleMessageDirective()
    {
        var request = DeserializeMessageEnvelope("handle", level: 7);

        var response = await Dispatch(request);

        var directive = FirstDirective(response);
        directive.GetProperty("type").GetString().Should().Be("Alexa.Presentation.HTML.HandleMessage");
        directive.GetProperty("message").GetProperty("action").GetString().Should().Be("handle");
        directive.GetProperty("message").GetProperty("level").GetInt32().Should().Be(7);
    }

    [Fact]
    public async Task Send_MessageRequestWithStartAction_DispatchesAndSerializesStartDirective()
    {
        var request = DeserializeMessageEnvelope("start", level: 3);

        var response = await Dispatch(request);

        var directive = FirstDirective(response);
        directive.GetProperty("type").GetString().Should().Be("Alexa.Presentation.HTML.Start");
        directive.GetProperty("request").GetProperty("uri").GetString().Should().Be("https://example.com/game");
        directive.GetProperty("request").GetProperty("method").GetString().Should().Be("GET");
        directive.GetProperty("configuration").GetProperty("timeoutInSeconds").ValueKind.Should().Be(JsonValueKind.Number);
    }

    private static SkillRequest DeserializeMessageEnvelope(string action, int level)
    {
        var json =
            "{\"version\":\"1.0\"," +
            "\"session\":{\"new\":true,\"sessionId\":\"s1\",\"application\":{\"applicationId\":\"" + SkillId + "\"},\"user\":{\"userId\":\"u1\"}}," +
            "\"context\":{\"System\":{\"application\":{\"applicationId\":\"" + SkillId + "\"}," +
            "\"user\":{\"userId\":\"u1\"},\"device\":{\"deviceId\":\"d1\",\"supportedInterfaces\":{\"Alexa.Presentation.HTML\":{}}}}}," +
            "\"request\":{\"type\":\"Alexa.Presentation.HTML.Message\",\"requestId\":\"r1\"," +
            "\"timestamp\":\"2024-05-01T12:00:00Z\",\"locale\":\"en-US\"," +
            "\"message\":{\"action\":\"" + action + "\",\"level\":" + level + "}}}";

        return JsonSerializer.Deserialize<SkillRequest>(json, AlexaJsonOptions.DefaultOptions)!;
    }

    private async Task<SkillResponse> Dispatch(SkillRequest request)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Skill:SkillId"] = SkillId })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<SkillMediator>>(NullLogger<SkillMediator>.Instance);
        services.AddSingleton<ILogger<PerformanceLoggingBehavior>>(NullLogger<PerformanceLoggingBehavior>.Instance);
        services.AddSkillMediator(configuration, cfg =>
        {
            cfg.SkillId = SkillId;
            cfg.RegisterServicesFromAssemblyContaining<WebApiGamesMessageRequestHandler>();
        });
        // The interceptor generator registers every public IPipelineBehavior in this test assembly -
        // including TestKit's FakePipelineBehavior, whose (string, List<string>) constructor is not a DI
        // dependency. This test proves generated dispatch and response serialization, not pipeline
        // composition, so the pipeline behaviors are cleared (RequestHandlerWrapperImpl aggregates an
        // empty pipeline back to the handler delegate itself).
        services.RemoveAll<IPipelineBehavior>();
        services.AddScoped<SkillRequestFactory>(_ => () => request);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISkillMediator>();

        return await mediator.Send(request, CancellationToken);
    }

    private static JsonElement FirstDirective(SkillResponse response)
    {
        var json = JsonSerializer.Serialize(response, AlexaJsonOptions.DefaultOptions);
        using var document = JsonDocument.Parse(json);
        return document.RootElement.GetProperty("response").GetProperty("directives")[0].Clone();
    }
}
