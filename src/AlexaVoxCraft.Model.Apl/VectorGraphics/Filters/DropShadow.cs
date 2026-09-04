using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;

public class DropShadow : IAVGFilter
{
    [JsonPropertyName("type")] public string Type => nameof(DropShadow);

    [JsonPropertyName("color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Color { get; set; } = null!;

    [JsonPropertyName("horizontalOffset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> HorizontalOffset { get; set; } = null!;

    [JsonPropertyName("radius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> Radius { get; set; } = null!;

    [JsonPropertyName("verticalOffset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> VerticalOffset { get; set; } = null!;
}