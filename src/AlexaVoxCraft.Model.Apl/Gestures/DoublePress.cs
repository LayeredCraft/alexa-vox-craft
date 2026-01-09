using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public class DoublePress : APLGesture
{
    [JsonPropertyName("type")]
    public override string Type => nameof(DoublePress);

    [JsonPropertyName("onDoublePress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnDoublePress { get; set; }

    [JsonPropertyName("onSinglePress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnSinglePress { get; set; }
}