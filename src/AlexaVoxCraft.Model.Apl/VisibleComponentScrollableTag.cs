using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class VisibleComponentScrollableTag
{
    [JsonPropertyName("direction")]
    public ScrollableTagDirection Direction { get; set; }

    [JsonPropertyName("allow_forward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowForward { get; set; }

    [JsonPropertyName("allow_backward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowBackward { get; set; }
}