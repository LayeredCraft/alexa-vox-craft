using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaButton : APLComponent, IJsonSerializable<AlexaButton>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaButton);

    [JsonPropertyName("buttonStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonStyle { get; set; } = null!;

    [JsonPropertyName("buttonText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ButtonText { get; set; } = null!;

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> PrimaryAction { get; set; } = null!;

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Theme { get; set; } = null!;

    [JsonPropertyName("touchForward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> TouchForward { get; set; } = null!;

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Lang { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AlexaButton
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            primaryActionProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}