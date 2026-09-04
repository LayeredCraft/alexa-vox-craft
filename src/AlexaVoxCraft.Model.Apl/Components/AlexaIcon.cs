using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaIcon : APLComponent, IJsonSerializable<AlexaIcon>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaIcon);

    [JsonPropertyName("iconName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> IconName { get; set; } = null!;

    [JsonPropertyName("iconSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> IconSource { get; set; } = null!;

    [JsonPropertyName("iconSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue IconSize { get; set; } = null!;

    [JsonPropertyName("iconColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> IconColor { get; set; } = null!;

    [JsonPropertyName("iconStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> IconStyle { get; set; } = null!;

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Lang { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AlexaIcon
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}