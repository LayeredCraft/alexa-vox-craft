using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaPhoto : ResponsiveTemplate, IJsonSerializable<AlexaPhoto>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaPhoto);

    [JsonPropertyName("attributionImage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? AttributionImage { get; set; }

    [JsonPropertyName("buttonStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ButtonStyle { get; set; }

    [JsonPropertyName("buttonText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ButtonText { get; set; }

    [JsonPropertyName("headerTitleCanUseTwoLines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HeaderTitleCanUseTwoLines { get; set; }

    [JsonPropertyName("imageAccessibilityLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageAccessibilityLabel { get; set; }

    [JsonPropertyName("imageHideScrim")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageHideScrim { get; set; }

    [JsonPropertyName("imageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageSource { get; set; }

    [JsonPropertyName("primaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? PrimaryText { get; set; }

    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryText { get; set; }

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? PrimaryAction { get; set; }

    [JsonPropertyName("touchForward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? TouchForward { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaPhoto
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
    }
}