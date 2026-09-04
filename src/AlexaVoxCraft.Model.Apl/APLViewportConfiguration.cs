using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLViewportConfiguration
{
    [JsonPropertyName("video")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VideoSupport Video { get; set; } = null!;

    [JsonPropertyName("size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLViewportSize Size { get; set; } = null!;
}