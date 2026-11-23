using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class MessageRequestTests : TestBase<MessageRequestTests>
{
    [Fact]
    public void EnvelopePreservesRequestMetadata()
    {
        var envelope = JsonSerializer.Deserialize<SkillRequest>(Fx("Requests/Message_Envelope.json"), AlexaJson)!;

        var request = envelope.Request.Should().BeOfType<MessageRequest>().Subject;
        request.Type.Should().Be("Alexa.Presentation.HTML.Message");
        request.RequestId.Should().Be("amzn1.echo-api.request.0123456789");
        request.Locale.Should().Be("en-US");
        request.Timestamp.Should().Be(new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc));
        request.Message.GetProperty("action").GetString().Should().Be("start");
        request.Message.GetProperty("level").GetInt32().Should().Be(3);
    }

    [Theory]
    [InlineData("""{"a":1,"b":[true,null,"x"]}""", JsonValueKind.Object)]
    [InlineData("[1,2,3]", JsonValueKind.Array)]
    [InlineData("\"hello\"", JsonValueKind.String)]
    [InlineData("42", JsonValueKind.Number)]
    [InlineData("true", JsonValueKind.True)]
    [InlineData("null", JsonValueKind.Null)]
    public void PreservesInboundMessageOfAnyKind(string messageJson, JsonValueKind expectedKind)
    {
        var request = DeserializeRequest(messageJson);

        request.Message.ValueKind.Should().Be(expectedKind);
        JsonNode.DeepEquals(JsonNode.Parse(request.Message.GetRawText()), JsonNode.Parse(messageJson)).Should().BeTrue();
    }

    [Fact]
    public void PreservesUnicodeAndNesting()
    {
        var request = DeserializeRequest("""{"name":"José — 日本語 — 🎮","nested":{"deep":[1,{"x":"y"}]}}""");

        request.Message.GetProperty("name").GetString().Should().Be("José — 日本語 — 🎮");
        request.Message.GetProperty("nested").GetProperty("deep")[1].GetProperty("x").GetString().Should().Be("y");
    }

    [Fact]
    public void MissingMessageIsUndefined()
    {
        var request = DeserializeRequest(messageJson: null);

        request.Message.ValueKind.Should().Be(JsonValueKind.Undefined);
    }

    [Fact]
    public void MalformedJsonThrows()
    {
        const string json = """{"version":"1.0","request":{"type":"Alexa.Presentation.HTML.Message","requestId":"r","timestamp":"2024-05-01T12:00:00Z","locale":"en-US","message":{"a":}}}""";

        var act = () => JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void MessageSurvivesSourceDocumentDisposal()
    {
        MessageRequest request;
        using (var doc = JsonDocument.Parse(Fx("Requests/Message_Envelope.json")))
        {
            var envelope = JsonSerializer.Deserialize<SkillRequest>(doc.RootElement.GetRawText(), AlexaJson)!;
            request = envelope.Request.Should().BeOfType<MessageRequest>().Subject;
        }

        request.Message.GetProperty("action").GetString().Should().Be("start");
    }

    [Fact]
    public void GetMessageDeserializesWithCallerMetadata()
    {
        var request = DeserializeRequest("""{"Level":4}""");

        var data = request.GetMessage(WebApiGamesTestJsonContext.Default.GameData);

        data!.Level.Should().Be(4);
    }

    [Fact]
    public void GetMessageThrowsWhenMessageMissing()
    {
        var request = DeserializeRequest(messageJson: null);

        var act = () => request.GetMessage(WebApiGamesTestJsonContext.Default.GameData);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetMessageThrowsOnMismatch()
    {
        var request = DeserializeRequest("\"not-an-object\"");

        var act = () => request.GetMessage(WebApiGamesTestJsonContext.Default.GameData);

        act.Should().Throw<JsonException>();
    }

    private static MessageRequest DeserializeRequest(string? messageJson)
    {
        var messageProperty = messageJson is null ? string.Empty : $",\"message\":{messageJson}";
        var json =
            "{\"version\":\"1.0\",\"request\":{\"type\":\"Alexa.Presentation.HTML.Message\"," +
            "\"requestId\":\"r1\",\"timestamp\":\"2024-05-01T12:00:00Z\",\"locale\":\"en-US\"" +
            messageProperty + "}}";

        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson)!;
        return envelope.Request.Should().BeOfType<MessageRequest>().Subject;
    }
}
