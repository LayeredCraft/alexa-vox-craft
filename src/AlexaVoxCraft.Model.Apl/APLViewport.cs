using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLViewport : Viewport
{
    public const string ViewportType = "APL";
    [JsonPropertyName("type")] public override string Type => ViewportType;

    [JsonPropertyName("shape")] public ViewportShape Shape { get; set; }

    [JsonPropertyName("dpi")] public int DPI { get; set; }

    [JsonPropertyName("presentationType")] public string PresentationType { get; set; }

    [JsonPropertyName("canRotate")] public bool CanRotate { get; set; }

    [JsonPropertyName("configuration")] public APLViewportConfigurationContainer Configuration { get; set; }
}