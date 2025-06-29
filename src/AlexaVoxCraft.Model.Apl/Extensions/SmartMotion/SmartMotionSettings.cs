using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;

public class SmartMotionSettings
{
    [JsonPropertyName("deviceStateName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string DeviceStateName { get; set; }

    [JsonPropertyName("wakeWordResponse")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public WakeWordResponse? WakeWordResponse { get; set; }
}