using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Apl;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class WebApiGamesSupportTests : TestBase<WebApiGamesSupportTests>
{
    private static readonly Uri GameUri = new("https://example.com/game");

    [Fact]
    public void AddIsIdempotentWhenRepeated()
    {
        for (var i = 0; i < 50; i++)
        {
            WebApiGamesSupport.Add();
        }

        var directive = JsonSerializer.Deserialize<IDirective>(Fx("Directives/Start_Minimal.json"), AlexaJson);
        directive.Should().BeOfType<StartDirective>();

        var request = DeserializeMessageRequest();
        request.Should().BeOfType<MessageRequest>();
    }

    [Fact]
    public void SkillRequestWithMessageRequestRoundTrips()
    {
        var envelope = JsonSerializer.Deserialize<SkillRequest>(Fx("Requests/Message_Envelope.json"), AlexaJson)!;

        var json = JsonSerializer.Serialize(envelope, AlexaJson);
        var roundTripped = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson)!;

        var request = roundTripped.Request.Should().BeOfType<MessageRequest>().Subject;
        request.RequestId.Should().Be("amzn1.echo-api.request.0123456789");
        request.Message.GetProperty("action").GetString().Should().Be("start");
    }

    [Fact]
    public void SkillResponseWithDirectivesRoundTripsThroughResponseBuilder()
    {
        var response = ResponseBuilder.Empty();
        response.Response.Directives.Add(new StartDirective(new StartRequest(GameUri)));
        response.Response.Directives.Add(HandleMessageDirective.Create(
            new GameData { Level = 2 },
            WebApiGamesTestJsonContext.Default.GameData));

        var json = JsonSerializer.Serialize(response, AlexaJson);
        var roundTripped = JsonSerializer.Deserialize<SkillResponse>(json, AlexaJson)!;

        roundTripped.Response.Directives.Should().HaveCount(2);
        roundTripped.Response.Directives[0].Should().BeOfType<StartDirective>();
        roundTripped.Response.Directives[1].Should().BeOfType<HandleMessageDirective>();
    }

    [Fact]
    public void APLSkillRequestRoundTripsWithAPLRegistered()
    {
        var envelope = JsonSerializer.Deserialize<APLSkillRequest>(Fx("Requests/APLUserEvent.json"), AlexaJson)!;
        envelope.Request.Should().BeOfType<UserEventRequest>();

        var json = JsonSerializer.Serialize(envelope, AlexaJson);
        var roundTripped = JsonSerializer.Deserialize<APLSkillRequest>(json, AlexaJson)!;

        roundTripped.Request.Should().BeOfType<UserEventRequest>();
    }

    [Fact]
    public void UnknownRequestTypeStillThrows()
    {
        const string json =
            """{"version":"1.0","request":{"type":"Totally.Unknown","requestId":"r","timestamp":"2024-05-01T12:00:00Z","locale":"en-US"}}""";

        var act = () => JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void UnknownDirectiveStillFallsBackToJsonDirective()
    {
        const string json = """{"type":"Totally.Unknown","foo":"bar"}""";

        var directive = JsonSerializer.Deserialize<IDirective>(json, AlexaJson);

        directive.Should().BeOfType<JsonDirective>();
    }

    private static MessageRequest DeserializeMessageRequest()
    {
        var envelope = JsonSerializer.Deserialize<SkillRequest>(Fx("Requests/Message_Envelope.json"), AlexaJson)!;
        return envelope.Request.Should().BeOfType<MessageRequest>().Subject;
    }
}
