using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public class AVGText : AVGItem, IJsonSerializable<AVGText>
{
    [JsonPropertyName("type")] public override string Type => "text";

    [JsonPropertyName("fontFamily")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontFamily { get; set; }

    [JsonPropertyName("fontSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> FontSize { get; set; }

    [JsonPropertyName("fontStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontStyle { get; set; }

    [JsonPropertyName("fontWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontWeight { get; set; }

    [JsonPropertyName("fillOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?> FillOpacity { get; set; }

    [JsonPropertyName("fill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Fill { get; set; }

    [JsonPropertyName("fillTransform")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FillTransform { get; set; }

    [JsonPropertyName("strokeOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?> StrokeOpacity { get; set; }

    [JsonPropertyName("stroke")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Stroke { get; set; }

    [JsonPropertyName("strokeWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> StrokeWidth { get; set; }

    [JsonPropertyName("strokeTransform")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> StrokeTransform { get; set; }

    [JsonPropertyName("x")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> X { get; set; }

    [JsonPropertyName("y")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> Y { get; set; }

    [JsonPropertyName("style")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Style { get; set; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Text { get; set; }

    [JsonPropertyName("textAnchor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> TextAnchor { get; set; }

    [JsonPropertyName("letterSpacing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> LetterSpacing { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AVGText
    {
        AVGItem.RegisterTypeInfo<T>();
    }
}