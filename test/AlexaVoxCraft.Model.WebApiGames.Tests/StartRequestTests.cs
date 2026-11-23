using Compono.XunitV3;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class StartRequestTests
{
    [Fact]
    public void KeepsUriAndAlwaysUsesGet()
    {
        var request = new StartRequest(new Uri("https://example.com/game"));

        request.Uri.Should().Be(new Uri("https://example.com/game"));
        request.Method.Should().Be("GET");
        request.Headers.Should().BeNull();
    }

    [Fact]
    public void PreservesProvidedHeaders()
    {
        var headers = new Dictionary<string, string> { ["X-Game"] = "duel" };

        var request = new StartRequest(new Uri("https://example.com/game"), headers);

        request.Headers.Should().BeEquivalentTo(headers);
    }

    [Theory]
    [Compose]
    public void PreservesIncidentalHeaders(Dictionary<string, string> headers)
    {
        var request = new StartRequest(new Uri("https://example.com/game"), headers);

        request.Headers.Should().BeEquivalentTo(headers);
    }

    [Fact]
    public void RejectsNullUri()
    {
        var act = () => new StartRequest(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RejectsRelativeUri()
    {
        var act = () => new StartRequest(new Uri("/game", UriKind.Relative));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("http://example.com/game")]
    [InlineData("ftp://example.com/game")]
    public void RejectsNonHttpsUri(string uri)
    {
        var act = () => new StartRequest(new Uri(uri));

        act.Should().Throw<ArgumentException>();
    }
}
