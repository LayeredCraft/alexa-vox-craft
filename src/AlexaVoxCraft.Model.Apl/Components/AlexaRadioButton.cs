using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaRadioButton : APLComponent, IJsonSerializable<AlexaRadioButton>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaRadioButton);

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? PrimaryAction { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("radioButtonColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? RadioButtonColor { get; set; }

    [JsonPropertyName("radioButtonHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? RadioButtonHeight { get; set; }

    [JsonPropertyName("radioButtonWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? RadioButtonWidth { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaRadioButton
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}