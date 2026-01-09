using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaButton : APLComponent, IJsonSerializable<AlexaButton>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaButton);

    [JsonPropertyName("buttonStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonStyle { get; set; }

    [JsonPropertyName("buttonText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonText { get; set; }

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> PrimaryAction { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Theme { get; set; }

    [JsonPropertyName("touchForward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> TouchForward { get; set; }

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Lang { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaButton
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}