using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.DataStore.PackageManager;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Apl;

public class APLContext : Context
{
    [JsonPropertyName("Display")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AlexaDisplay? Display { get; set; }

    [JsonPropertyName("Viewport")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AlexaViewport? Viewport { get; set; }

    [JsonPropertyName("Viewports")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Viewport[]? Viewports { get; set; }

    [JsonPropertyName("Extensions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AlexaExtensions? Extensions { get; set; }

    [JsonPropertyName("Alexa.Presentation.APL")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AplVisualContext? AplVisualContext { get; set; }

    [JsonPropertyName("Alexa.DataStore.PackageManager")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PackageManagerContext? PackageManagerContext { get; set; }
}