using System.Text.Json;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class HandleMessageDirectiveTests : TestBase<HandleMessageDirectiveTests>
{
    [Fact]
    public void MinimalWireJsonMatchesFixture()
    {
        using var message = JsonDocument.Parse("""{"level":3}""");
        var directive = new HandleMessageDirective(message.RootElement);

        var json = TestHelper.Serialize(directive);

        TestHelper.AssertJsonMatches(json, Fx("Directives/HandleMessage_Minimal.json"));

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        root.GetProperty("type").GetString().Should().Be("Alexa.Presentation.HTML.HandleMessage");
        root.GetProperty("message").ValueKind.Should().Be(JsonValueKind.Object);
        root.TryGetProperty("transformers", out _).Should().BeFalse();
    }

    [Fact]
    public void TransformersWireJsonMatchesFixture()
    {
        using var message = JsonDocument.Parse("""{"level":3}""");
        var directive = new HandleMessageDirective(
            message.RootElement,
            [HTMLTransformer.SsmlToSpeech("card/body")]);

        var json = TestHelper.Serialize(directive);

        TestHelper.AssertJsonMatches(json, Fx("Directives/HandleMessage_WithTransformers.json"));
    }

    [Fact]
    public void DeserializesViaDirectiveConverter()
    {
        var directive = JsonSerializer.Deserialize<IDirective>(Fx("Directives/HandleMessage_WithTransformers.json"), AlexaJson);

        var handle = directive.Should().BeOfType<HandleMessageDirective>().Subject;
        handle.Message.GetProperty("level").GetInt32().Should().Be(3);
        handle.Transformers.Should().ContainSingle().Which.Transformer.Should().Be("ssmlToSpeech");
    }

    [Theory]
    [InlineData("42")]
    [InlineData("\"text\"")]
    [InlineData("[1,2]")]
    [InlineData("true")]
    [InlineData("null")]
    public void RejectsNonObjectMessage(string messageJson)
    {
        using var doc = JsonDocument.Parse(messageJson);

        var act = () => new HandleMessageDirective(doc.RootElement);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MessageSurvivesSourceDocumentDisposal()
    {
        HandleMessageDirective directive;
        using (var doc = JsonDocument.Parse("""{"level":9}"""))
        {
            directive = new HandleMessageDirective(doc.RootElement);
        }

        var json = TestHelper.Serialize(directive);

        using var parsed = JsonDocument.Parse(json);
        parsed.RootElement.GetProperty("message").GetProperty("level").GetInt32().Should().Be(9);
    }

    [Fact]
    public void CreateSerializesCallerTypedMessage()
    {
        var directive = HandleMessageDirective.Create(
            new GameData { Level = 5 },
            WebApiGamesTestJsonContext.Default.GameData);

        var element = TestHelper.SerializeToElement(directive);

        element.GetProperty("message").GetProperty("Level").GetInt32().Should().Be(5);
    }
}
