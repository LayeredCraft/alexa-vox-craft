using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public class SwipeAway : APLGesture
{
    [JsonPropertyName("type")] public override string Type => nameof(SwipeAway);

    [JsonPropertyName("onSwipeMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnSwipeMove { get; set; }

    [JsonPropertyName("onSwipeDone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnSwipeDone { get; set; }

    [JsonPropertyName("item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public APLValueCollection<APLComponent>? Item { get; set; }

    [JsonPropertyName("action")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SwipeAction?>? Action { get; set; }

    [JsonPropertyName("direction")] public APLValue<SwipeDirection> Direction { get; set; }
}