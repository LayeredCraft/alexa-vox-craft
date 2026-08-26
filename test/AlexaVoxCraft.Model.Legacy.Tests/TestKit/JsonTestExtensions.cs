using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.TestKit.Extensions;

public static class JsonTestExtensions
{
    public static void ShouldRoundTripSerialize<T>(this T actual)
    {
        var originalJson = JsonSerializer.Serialize(actual, AlexaJsonOptions.DefaultOptions);
        var deserialized = JsonSerializer.Deserialize<T>(originalJson, AlexaJsonOptions.DefaultOptions);
        var deserializedJson = JsonSerializer.Serialize(deserialized, AlexaJsonOptions.DefaultOptions);

        deserializedJson.Should().Be(originalJson);
    }
}
