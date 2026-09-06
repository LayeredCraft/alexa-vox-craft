using System.Text.Json;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Coverage for <see cref="JsonDirective"/>, the fallback container for directive types the SDK
/// doesn't know about. Unlike a normal response directive it is genuinely used both ways in this
/// SDK: consumers can deserialize an unknown directive back out of extension data as well as
/// construct and serialize one, so (per the Constraints rule) it is tested both directions.
/// </summary>
public sealed class JsonDirectiveTests() : TestBase<JsonDirectiveTests>
{
    [Fact]
    public void UnknownDirectiveType_RoundTrips()
    {
        var directive = new JsonDirective("UnknownDirectiveType");
        var nested = JsonSerializer.Deserialize<JsonElement>("""{ "value": "test" }""");
        directive.Properties["testProperty"] = nested;

        var options = new JsonSerializerOptions { WriteIndented = false };
        var json = JsonSerializer.Serialize<IDirective>(directive, options);
        var roundTripped = JsonSerializer.Deserialize<IDirective>(json, options);

        var result = roundTripped.Should().BeOfType<JsonDirective>().Subject;
        result.Type.Should().Be("UnknownDirectiveType");
        result.Properties.Should().ContainKey("testProperty");
        result.Properties["testProperty"].ValueKind.Should().Be(JsonValueKind.Object);
        result.Properties["testProperty"].GetProperty("value").GetString().Should().Be("test");
    }

    [Fact]
    public void EmptyDirectiveOrNoOverride_UsesSetValue()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        tell.Response.ShouldEndSession.Should().BeTrue();

        tell.Response.Directives.Add(new JsonDirective("nothingspecial"));
        tell.Response.ShouldEndSession.Should().BeTrue();
    }
}
