using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public class AVGPath : AVGItem
{
    [JsonPropertyName("type")] public override string Type => "path";

    [JsonPropertyName("fillOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? FillOpacity { get; set; }

    [JsonPropertyName("fill")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Fill { get; set; }

    [JsonPropertyName("fillTransform")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FillTransform { get; set; }

    [JsonPropertyName("pathData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? PathData { get; set; }

    [JsonPropertyName("pathLength")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? PathLength { get; set; }

    [JsonPropertyName("stroke")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Stroke { get; set; }

    [JsonPropertyName("strokeDashArray")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLValue<int?>>? StrokeDashArray { get; set; }

    [JsonPropertyName("strokeDashOffset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? StrokeDashOffset { get; set; }

    [JsonPropertyName("strokeLineCap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<StrokeLineCap?>? StrokeLineCap { get; set; }

    [JsonPropertyName("strokeLineJoin")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<StrokeLineJoin?>? StrokeLineJoin { get; set; }

    [JsonPropertyName("strokeMiterLimit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? StrokeMiterLimit { get; set; }

    [JsonPropertyName("strokeOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? StrokeOpacity { get; set; }

    [JsonPropertyName("strokeWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? StrokeWidth { get; set; }

    [JsonPropertyName("strokeTransform")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? StrokeTransform { get; set; }

    [JsonPropertyName("style")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Style { get; set; }
}

[System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<StrokeLineJoin>))]
public enum StrokeLineJoin
{
    [EnumMember(Value = "bevel")] Bevel,
    [EnumMember(Value = "miter")] Miter,
    [EnumMember(Value = "round")] Round
}

[System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<StrokeLineCap>))]
public enum StrokeLineCap
{
    [EnumMember(Value = "butt")] Butt,
    [EnumMember(Value = "round")] Round,
    [EnumMember(Value = "square")] Square
}