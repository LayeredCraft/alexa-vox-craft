using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class AlexaViewport
{
    [JsonPropertyName("experiences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ViewportExperience[]? Experiences { get; set; }

    [JsonPropertyName("currentPixelWidth")]
    public int CurrentPixelWidth { get; set; }

    [JsonPropertyName("currentPixelHeight")]
    public int CurrentPixelHeight { get; set; }

    [JsonPropertyName("dpi")] public int DPI { get; set; }

    [JsonPropertyName("pixelHeight")] public int PixelHeight { get; set; }

    [JsonPropertyName("pixelWidth")] public int PixelWidth { get; set; }

    [JsonPropertyName("touch")] public string[] Touch { get; set; }

    [JsonPropertyName("keyboard")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Keyboard { get; set; }

    [JsonPropertyName("shape")] public ViewportShape Shape { get; set; }

    [JsonPropertyName("video")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VideoSupport? Video { get; set; }
}