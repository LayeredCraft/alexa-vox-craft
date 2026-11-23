using System.Text.Json;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

/// <summary>
/// Proves the typed payload helpers consume the caller-supplied <see cref="System.Text.Json.Serialization.Metadata.JsonTypeInfo{T}"/>
/// rather than any ambient metadata: a second caller context with a camelCase naming policy changes the
/// serialized payload shape.
/// </summary>
public sealed class TypedPayloadHelpersTests : TestBase<TypedPayloadHelpersTests>
{
    [Fact]
    public void StartDirective_Create_UsesCallerMetadata()
    {
        var directive = StartDirective.Create(
            new StartRequest(new Uri("https://example.com/game")),
            new CamelCaseGameData { Score = 11 },
            CamelCaseTestJsonContext.Default.CamelCaseGameData);

        var element = TestHelper.SerializeToElement(directive);

        element.GetProperty("data").GetProperty("score").GetInt32().Should().Be(11);
    }

    [Fact]
    public void HandleMessageDirective_Create_UsesCallerMetadata()
    {
        var directive = HandleMessageDirective.Create(
            new CamelCaseGameData { Score = 12 },
            CamelCaseTestJsonContext.Default.CamelCaseGameData);

        var element = TestHelper.SerializeToElement(directive);

        element.GetProperty("message").GetProperty("score").GetInt32().Should().Be(12);
    }

    [Fact]
    public void MessageRequest_GetMessage_UsesCallerMetadata()
    {
        const string json =
            """{"version":"1.0","request":{"type":"Alexa.Presentation.HTML.Message","requestId":"r","timestamp":"2024-05-01T12:00:00Z","locale":"en-US","message":{"score":13}}}""";
        var envelope = JsonSerializer.Deserialize<AlexaVoxCraft.Model.Request.SkillRequest>(json, AlexaJson)!;
        var request = envelope.Request.Should().BeOfType<MessageRequest>().Subject;

        var data = request.GetMessage(CamelCaseTestJsonContext.Default.CamelCaseGameData);

        data!.Score.Should().Be(13);
    }
}
