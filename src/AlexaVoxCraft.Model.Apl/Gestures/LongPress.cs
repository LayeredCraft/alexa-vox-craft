using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public class LongPress : APLGesture
{
    [JsonPropertyName("type")]
    public override string Type => nameof(LongPress);

    [JsonPropertyName("onLongPressStart")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnLongPressStart { get; set; }

    [JsonPropertyName("onLongPressEnd")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnLongPressEnd { get; set; }
}