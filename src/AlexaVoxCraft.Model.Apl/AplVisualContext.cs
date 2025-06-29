using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class AplVisualContext
{
    [JsonPropertyName("presentationUri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PresentationUri { get; set; }

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }

    [JsonPropertyName("version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Version { get; set; }

    [JsonPropertyName("componentsVisibleOnScreen")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VisibleComponent[]? ComponentsVisibleOnScreen { get; set; }
}