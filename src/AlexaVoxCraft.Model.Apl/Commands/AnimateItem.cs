using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class AnimateItem : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(AnimateItem);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("duration")] public APLValue<int?>? Duration { get; set; }

    [JsonPropertyName("easing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Easing { get; set; }

    [JsonPropertyName("repeatCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? RepeatCount { get; set; }

    [JsonPropertyName("repeatMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RepeatMode>? RepeatMode { get; set; }

    [JsonPropertyName("value")] public APLValueCollection<AnimatedProperty> Value { get; set; }

}