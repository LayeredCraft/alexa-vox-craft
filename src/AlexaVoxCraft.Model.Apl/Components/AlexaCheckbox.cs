using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaCheckbox : APLComponent, IJsonSerializable<AlexaCheckbox>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaCheckbox);

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> PrimaryAction { get; set; } = null!;

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Theme { get; set; } = null!;

    [JsonPropertyName("selectedColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> SelectedColor { get; set; } = null!;

    [JsonPropertyName("checkboxHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue CheckboxHeight { get; set; } = null!;

    [JsonPropertyName("checkboxWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue CheckboxWidth { get; set; } = null!;

    [JsonPropertyName("isIndeterminate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> IsIndeterminate { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AlexaCheckbox
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            primaryActionProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}