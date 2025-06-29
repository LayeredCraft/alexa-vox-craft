using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class GeolocationSpeed
{
    [JsonPropertyName("speedInMetersPerSecond")]
    public double? Speed { get; set; }
    [JsonPropertyName("accuracyInMetresPerSecond")]
    public double? Accuracy { get; set; }
}