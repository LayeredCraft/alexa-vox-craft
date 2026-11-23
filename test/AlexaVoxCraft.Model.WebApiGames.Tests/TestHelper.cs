using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

internal static class TestHelper
{
    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, AlexaJsonOptions.DefaultOptions);

    public static JsonElement SerializeToElement<T>(T value) => JsonSerializer.SerializeToElement(value, AlexaJsonOptions.DefaultOptions);

    public static void AssertJsonMatches(string actual, string expectedJson)
    {
        var actualNode = JsonNode.Parse(actual);
        var expectedNode = JsonNode.Parse(expectedJson);

        JsonNode.DeepEquals(actualNode, expectedNode).Should().BeTrue(
            $"expected serialized JSON to match the fixture.\nactual:   {actual}\nexpected: {expectedJson}");
    }
}
