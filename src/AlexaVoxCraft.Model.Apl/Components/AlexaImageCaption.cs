using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaImageCaption : ResponsiveTemplate, IJsonSerializable<AlexaImageCaption>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaImageCaption);

    [JsonPropertyName("attributionImage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> AttributionImage { get; set; } = null!;

    [JsonPropertyName("buttonStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonStyle { get; set; } = null!;

    [JsonPropertyName("buttonText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonText { get; set; } = null!;

    [JsonPropertyName("headerTitleCanUseTwoLines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderTitleCanUseTwoLines { get; set; } = null!;

    [JsonPropertyName("imageAccessibilityLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ImageAccessibilityLabel { get; set; } = null!;

    [JsonPropertyName("imageScrim")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> ImageScrim { get; set; } = null!;

    [JsonPropertyName("imageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ImageSource { get; set; } = null!;

    [JsonPropertyName("primaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> PrimaryText { get; set; } = null!;

    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> SecondaryText { get; set; } = null!;

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> PrimaryAction { get; set; } = null!;

    [JsonPropertyName("touchForward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> TouchForward { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AlexaImageCaption
    {
        ResponsiveTemplate.RegisterTypeInfo<ResponsiveTemplate>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            primaryActionProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}