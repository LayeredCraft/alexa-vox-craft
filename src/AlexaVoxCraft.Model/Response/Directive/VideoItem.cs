using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class VideoItem
{
    public VideoItem(string source)
    {
        Source = source;
    }

    [JsonPropertyName("source")]
    [JsonRequired]
    public string Source { get; set; }

    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VideoItemMetadata Metadata { get; set; }
}