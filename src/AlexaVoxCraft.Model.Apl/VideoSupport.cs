using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class VideoSupport
{
    [JsonPropertyName("codecs")]
    public string[] Codecs { get; set; }
}