using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class VisibleComponentPagerTag
{
    [JsonPropertyName("index")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Index { get; set; }

    [JsonPropertyName("page_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PageCount { get; set; }

    [JsonPropertyName("allow_forward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowForward { get; set; }

    [JsonPropertyName("allow_backwards")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowBackwards { get; set; }
}