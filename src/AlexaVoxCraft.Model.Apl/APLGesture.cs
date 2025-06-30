using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(APLGestureConverter))]
public abstract class APLGesture
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}