using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaSwitch : APLComponent, IJsonSerializable<AlexaSwitch>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaSwitch);

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? PrimaryAction { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("activeColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ActiveColor { get; set; }

    [JsonPropertyName("switchHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? SwitchHeight { get; set; }

    [JsonPropertyName("switchWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? SwitchWidth { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaSwitch
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}