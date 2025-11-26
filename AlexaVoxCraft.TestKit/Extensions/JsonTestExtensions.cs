using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;
using AwesomeAssertions;

namespace AlexaVoxCraft.TestKit.Extensions;

public static class JsonTestExtensions
{
    public static void ShouldRoundTripSerialize<T>(this T actual)
    {
        // Serialize to JSON
        var originalJson = JsonSerializer.Serialize(actual, AlexaJsonOptions.DefaultOptions);
        
        // Deserialize back to object
        var deserialized = JsonSerializer.Deserialize<T>(originalJson, AlexaJsonOptions.DefaultOptions);
        
        // Verify they match
        var deserializedJson = JsonSerializer.Serialize(deserialized, AlexaJsonOptions.DefaultOptions);
        
        deserializedJson.Should().Be(originalJson);
    }
}