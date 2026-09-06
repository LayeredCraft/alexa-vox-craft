using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaTextWrapping : ResponsiveTemplate, IJsonSerializable<AlexaTextWrapping>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaTextWrapping);

    [JsonPropertyName("buttonStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonStyle { get; set; } = null!;

    [JsonPropertyName("buttonText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonText { get; set; } = null!;

    [JsonPropertyName("headerTitleCanUseTwoLines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderTitleCanUseTwoLines { get; set; } = null!;

    [JsonPropertyName("primaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> PrimaryText { get; set; } = null!;

    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> SecondaryText { get; set; } = null!;

    [JsonPropertyName("tertiaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> TertiaryText { get; set; } = null!;

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> PrimaryAction { get; set; } = null!;

    [JsonPropertyName("touchForward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> TouchForward { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AlexaTextWrapping
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            primaryActionProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}