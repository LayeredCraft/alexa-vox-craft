using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Scenario 1 (original console app): core Alexa JSON - deserialize a real LaunchRequest, serialize a
// response with a card and a directive (exercises the polymorphic converters).
public sealed class CoreJsonTests
{
    private const string LaunchJson = """
        {
          "version": "1.0",
          "session": { "new": true, "sessionId": "s1", "application": { "applicationId": "amzn1.ask.skill.validation" }, "user": { "userId": "u1" } },
          "context": {
            "System": { "application": { "applicationId": "amzn1.ask.skill.validation" }, "user": { "userId": "u1" }, "device": { "deviceId": "d1", "supportedInterfaces": {} }, "apiEndpoint": "https://api.amazonalexa.com", "apiAccessToken": "token" }
          },
          "request": { "type": "LaunchRequest", "requestId": "r1", "timestamp": "2026-01-01T00:00:00Z", "locale": "en-US" }
        }
        """;

    [Fact]
    public void Deserialize_RealLaunchRequest()
    {
        var request = JsonSerializer.Deserialize(LaunchJson, AlexaJsonTypeInfo.For<SkillRequest>());

        Assert.NotNull(request);
        Assert.IsType<LaunchRequest>(request.Request);
    }

    [Fact]
    public void Serialize_SkillResponse_WithCard()
    {
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

        var json = JsonSerializer.Serialize(response, AlexaJsonTypeInfo.For<SkillResponse>());

        Assert.Contains("\"card\"", json);
    }
}
