using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.WebApiGames;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Issue #213: reflection-disabled serialization/deserialization of the Web API for Games directive and
// request types through AlexaJsonOptions.DefaultOptions (no JIT fallback in this project), plus typed
// GetMessage<T> via this project's own source-generated metadata.
public sealed class WebApiGamesDirectiveTests
{
    private static readonly Uri GameUri = new("https://example.com/game");

    [Fact]
    public void Serialize_StartDirective_WritesExactWireFields()
    {
        var directive = new StartDirective(new StartRequest(GameUri));

        var json = JsonSerializer.Serialize(directive, AlexaJsonTypeInfo.For<StartDirective>());

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.Equal("Alexa.Presentation.HTML.Start", root.GetProperty("type").GetString());
        Assert.Equal("https://example.com/game", root.GetProperty("request").GetProperty("uri").GetString());
        Assert.Equal("GET", root.GetProperty("request").GetProperty("method").GetString());
        Assert.Equal(JsonValueKind.Number, root.GetProperty("configuration").GetProperty("timeoutInSeconds").ValueKind);
    }

    [Fact]
    public void Deserialize_StartDirective_ViaDirectiveConverter()
    {
        const string json =
            """{"type":"Alexa.Presentation.HTML.Start","request":{"uri":"https://example.com/game","method":"GET"},"configuration":{"timeoutInSeconds":30}}""";

        var directive = JsonSerializer.Deserialize(json, AlexaJsonTypeInfo.For<IDirective>());

        var start = Assert.IsType<StartDirective>(directive);
        Assert.Equal(GameUri, start.Request.Uri);
        Assert.Equal("GET", start.Request.Method);
        Assert.Equal(30, start.Configuration.TimeoutInSeconds);
    }

    [Fact]
    public void Serialize_HandleMessageDirective_WritesExactWireFields()
    {
        var directive = HandleMessageDirective.Create(
            new GameMessage { Action = "move", Level = 4 },
            ValidationAppContext.Default.GameMessage);

        var json = JsonSerializer.Serialize(directive, AlexaJsonTypeInfo.For<HandleMessageDirective>());

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.Equal("Alexa.Presentation.HTML.HandleMessage", root.GetProperty("type").GetString());
        Assert.Equal("move", root.GetProperty("message").GetProperty("action").GetString());
        Assert.Equal(4, root.GetProperty("message").GetProperty("level").GetInt32());
    }

    [Fact]
    public void Deserialize_HandleMessageDirective_ViaDirectiveConverter()
    {
        const string json =
            """{"type":"Alexa.Presentation.HTML.HandleMessage","message":{"action":"move","level":4}}""";

        var directive = JsonSerializer.Deserialize(json, AlexaJsonTypeInfo.For<IDirective>());

        var handle = Assert.IsType<HandleMessageDirective>(directive);
        Assert.Equal("move", handle.Message.GetProperty("action").GetString());
        Assert.Equal(4, handle.Message.GetProperty("level").GetInt32());
    }
}

public sealed class WebApiGamesMessageRequestTests
{
    [Fact]
    public void Deserialize_MessageRequest_PreservesObjectPayload_AndReadsTypedMessage()
    {
        var request = DeserializeRequest("""{"action":"start","level":3}""");

        Assert.Equal(JsonValueKind.Object, request.Message.ValueKind);

        var message = request.GetMessage(ValidationAppContext.Default.GameMessage);
        Assert.NotNull(message);
        Assert.Equal("start", message.Action);
        Assert.Equal(3, message.Level);
    }

    [Theory]
    [InlineData("[1,2,3]", JsonValueKind.Array)]
    [InlineData("\"hello\"", JsonValueKind.String)]
    [InlineData("42", JsonValueKind.Number)]
    [InlineData("true", JsonValueKind.True)]
    [InlineData("null", JsonValueKind.Null)]
    public void Deserialize_MessageRequest_PreservesNonObjectPayload(string messageJson, JsonValueKind expectedKind)
    {
        var request = DeserializeRequest(messageJson);

        Assert.Equal(expectedKind, request.Message.ValueKind);
    }

    [Fact]
    public void Deserialize_MessageRequest_WithoutMessage_LeavesMessageUndefined()
    {
        var request = DeserializeRequest(messageJson: null);

        Assert.Equal(JsonValueKind.Undefined, request.Message.ValueKind);
    }

    private static MessageRequest DeserializeRequest(string? messageJson)
    {
        var messageProperty = messageJson is null ? string.Empty : $",\"message\":{messageJson}";
        var json =
            "{\"version\":\"1.0\",\"request\":{\"type\":\"Alexa.Presentation.HTML.Message\"," +
            "\"requestId\":\"r1\",\"timestamp\":\"2024-05-01T12:00:00Z\",\"locale\":\"en-US\"" +
            messageProperty + "}}";

        var envelope = JsonSerializer.Deserialize(json, AlexaJsonTypeInfo.For<SkillRequest>());
        Assert.NotNull(envelope);
        return Assert.IsType<MessageRequest>(envelope.Request);
    }
}
