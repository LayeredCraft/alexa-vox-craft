using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class ViewportExperience
{
    [JsonPropertyName("canResize")]
    public bool CanResize { get; set; }

    [JsonPropertyName("canRotate")]
    public bool CanRotate { get; set; }

    [JsonPropertyName("arcMinuteWidth")]
    public int ArcMinuteWidth { get; set; }

    [JsonPropertyName("arcMinuteHeight")]
    public int ArcMinuteHeight { get; set; }
}