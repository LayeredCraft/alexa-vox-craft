using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public class AVGText : AVGItem, IJsonSerializable<AVGText>
{
    [JsonPropertyName("type")] public override string Type => "text";

    [JsonPropertyName("fontFamily")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontFamily { get; set; } = null!;

    [JsonPropertyName("fontSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> FontSize { get; set; } = null!;

    [JsonPropertyName("fontStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontStyle { get; set; } = null!;

    [JsonPropertyName("fontWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontWeight { get; set; } = null!;

    [JsonPropertyName("fillOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?> FillOpacity { get; set; } = null!;

    [JsonPropertyName("fill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Fill { get; set; } = null!;

    [JsonPropertyName("fillTransform")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FillTransform { get; set; } = null!;

    [JsonPropertyName("strokeOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?> StrokeOpacity { get; set; } = null!;

    [JsonPropertyName("stroke")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Stroke { get; set; } = null!;

    [JsonPropertyName("strokeWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> StrokeWidth { get; set; } = null!;

    [JsonPropertyName("strokeTransform")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> StrokeTransform { get; set; } = null!;

    [JsonPropertyName("x")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> X { get; set; } = null!;

    [JsonPropertyName("y")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> Y { get; set; } = null!;

    [JsonPropertyName("style")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Style { get; set; } = null!;

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Text { get; set; } = null!;

    [JsonPropertyName("textAnchor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> TextAnchor { get; set; } = null!;

    [JsonPropertyName("letterSpacing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> LetterSpacing { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AVGText
    {
        AVGItem.RegisterTypeInfo<T>();
    }
}