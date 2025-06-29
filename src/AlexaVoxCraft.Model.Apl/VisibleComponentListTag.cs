using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class VisibleComponentListTag
{
    [JsonPropertyName("item_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ItemCount { get; set; }

    [JsonPropertyName("lowest_index_seen")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LowestIndexSeen { get; set; }

    [JsonPropertyName("highest_index_seen")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? HighestIndexSeen { get; set; }

    [JsonPropertyName("lowest_ordinal_seen")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LowestOrdinalSeen { get; set; }

    [JsonPropertyName("highest_ordinal_seen")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? HighestOrdinalSeen { get; set; }
}