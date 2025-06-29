using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class VisibleComponentMediaTag
{
    [JsonPropertyName("position_in_milliseconds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PositionInMilliseconds { get; set; }

    [JsonPropertyName("allow_adjust_seek_position_forward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowAdjustSeekPositionForward { get; set; }

    [JsonPropertyName("allow_adjust_seek_position_backwards")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowAdjustSeekPositionBackwards { get; set; }

    [JsonPropertyName("allow_next")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowNext { get; set; }

    [JsonPropertyName("allow_previous")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowPrevious { get; set; }

    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; set; }

    [JsonPropertyName("state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MediaTagState? State { get; set; }

    [JsonPropertyName("entities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VisibleComponentEntity[]? Entities { get; set; }
}