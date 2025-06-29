using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLTViewport : Viewport
{
    public const string ViewportType = "APLT";
    [JsonPropertyName("type")]
    public override string Type => ViewportType;

    [JsonPropertyName("supportedProfiles")]
    public APLTProfile[] SupportedProfiles { get; set; }

    [JsonPropertyName("lineLength")]
    public int LineLength { get; set; }

    [JsonPropertyName("lineCount")]
    public int LineCount { get; set; }

    [JsonPropertyName("format")]
    public APLTFormat Format { get; set; }

    [JsonPropertyName("interSegments")]
    public InterSegment[] InterSegments { get; set; }
}