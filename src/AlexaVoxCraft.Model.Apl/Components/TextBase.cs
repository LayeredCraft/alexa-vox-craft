using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class TextBase : APLComponent, IJsonSerializable<TextBase>
{
    [JsonPropertyName("fontFamily")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FontFamily { get; set; }

    [JsonPropertyName("color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Color { get; set; }

    [JsonPropertyName("fontSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? FontSize { get; set; }

    [JsonPropertyName("fontStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FontStyle { get; set; }

    [JsonPropertyName("fontWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FontWeight { get; set; }

    [JsonPropertyName("letterSpacing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? LetterSpacing { get; set; }

    [JsonPropertyName("lineHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? LineHeight { get; set; }

    [JsonPropertyName("maxLines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? MaxLines { get; set; }

    [JsonPropertyName("textAlign")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TextAlign { get; set; }

    [JsonPropertyName("textAlignVertical")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TextAlignVertical { get; set; }

    [JsonPropertyName("overflow")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<TextOverflow?>? TextOverflow { get; set; }

    [JsonPropertyName("msPerCharacter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? MsPerCharacter { get; set; }

    public new static void RegisterTypeInfo<T>() where T : TextBase
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}