using System.Text.Json;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class StartDirectiveTests : TestBase<StartDirectiveTests>
{
    private static readonly Uri GameUri = new("https://example.com/game");

    [Fact]
    public void MinimalWireJsonMatchesFixture()
    {
        var directive = new StartDirective(new StartRequest(GameUri));

        var json = TestHelper.Serialize(directive);

        TestHelper.AssertJsonMatches(json, Fx("Directives/Start_Minimal.json"));

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        root.GetProperty("type").GetString().Should().Be("Alexa.Presentation.HTML.Start");
        root.GetProperty("request").GetProperty("uri").GetString().Should().Be("https://example.com/game");
        root.GetProperty("request").GetProperty("method").GetString().Should().Be("GET");
        root.GetProperty("request").TryGetProperty("headers", out _).Should().BeFalse();
        root.GetProperty("configuration").GetProperty("timeoutInSeconds").ValueKind.Should().Be(JsonValueKind.Number);
        root.TryGetProperty("data", out _).Should().BeFalse();
        root.TryGetProperty("transformers", out _).Should().BeFalse();
    }

    [Fact]
    public void FullWireJsonMatchesFixture()
    {
        using var data = JsonDocument.Parse("""{"level":3}""");
        var directive = new StartDirective(
            new StartRequest(GameUri, new Dictionary<string, string> { ["X-Token"] = "abc" }),
            new StartConfiguration(300),
            data.RootElement,
            [HTMLTransformer.TextToSpeech("card/title", "spokenTitle")]);

        var json = TestHelper.Serialize(directive);

        TestHelper.AssertJsonMatches(json, Fx("Directives/Start_Full.json"));
    }

    [Fact]
    public void ConfigurationDefaultsWhenNull()
    {
        var directive = new StartDirective(new StartRequest(GameUri));

        directive.Configuration.TimeoutInSeconds.Should().Be(30);
    }

    [Fact]
    public void DeserializesViaDirectiveConverter()
    {
        var directive = JsonSerializer.Deserialize<IDirective>(Fx("Directives/Start_Full.json"), AlexaJson);

        var start = directive.Should().BeOfType<StartDirective>().Subject;
        start.Request.Uri.Should().Be(GameUri);
        start.Request.Method.Should().Be("GET");
        start.Request.Headers!["X-Token"].Should().Be("abc");
        start.Configuration.TimeoutInSeconds.Should().Be(300);
        start.Data!.Value.GetProperty("level").GetInt32().Should().Be(3);
        start.Transformers.Should().ContainSingle().Which.Transformer.Should().Be("textToSpeech");
    }

    [Theory]
    [InlineData("42")]
    [InlineData("\"text\"")]
    [InlineData("[1,2]")]
    [InlineData("true")]
    [InlineData("null")]
    public void RejectsNonObjectData(string dataJson)
    {
        using var doc = JsonDocument.Parse(dataJson);

        var act = () => new StartDirective(new StartRequest(GameUri), data: doc.RootElement);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DataSurvivesSourceDocumentDisposal()
    {
        JsonElement element;
        using (var doc = JsonDocument.Parse("""{"level":9}"""))
        {
            element = doc.RootElement;
        }

        var directive = new StartDirective(new StartRequest(GameUri), data: element);

        var json = TestHelper.Serialize(directive);

        using var parsed = JsonDocument.Parse(json);
        parsed.RootElement.GetProperty("data").GetProperty("level").GetInt32().Should().Be(9);
    }

    [Fact]
    public void CreateSerializesCallerTypedData()
    {
        var directive = StartDirective.Create(
            new StartRequest(GameUri),
            new GameData { Level = 7 },
            WebApiGamesTestJsonContext.Default.GameData);

        var element = TestHelper.SerializeToElement(directive);

        element.GetProperty("data").GetProperty("Level").GetInt32().Should().Be(7);
    }
}
