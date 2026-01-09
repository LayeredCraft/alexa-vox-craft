using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class PlayMedia : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => "PlayMedia";

    [JsonPropertyName("audioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? AudioTrack { get; set; } = "foreground";

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("source")]
    public APLValueCollection<VideoSource> Value { get; set; }
}