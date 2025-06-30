using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLTransform
{
    [JsonPropertyName("rotate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? Rotate { get; set; }

    [JsonPropertyName("scale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? Scale { get; set; }

    [JsonPropertyName("scaleX")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? ScaleX { get; set; }

    [JsonPropertyName("scaleY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? ScaleY { get; set; }

    [JsonPropertyName("skewX")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? SkewX { get; set; }

    [JsonPropertyName("skewY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? SkewY { get; set; }

    [JsonPropertyName("translateX")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? TranslateX { get; set; }

    [JsonPropertyName("translateY")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? TranslateY { get; set; }
}