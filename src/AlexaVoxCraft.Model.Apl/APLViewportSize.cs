using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLViewportSize
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("pixelHeight")] public int PixelHeight { get; set; }

    [JsonPropertyName("pixelWidth")] public int PixelWidth { get; set; }
}