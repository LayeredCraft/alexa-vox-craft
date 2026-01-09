using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaIconButton : APLComponent, IJsonSerializable<AlexaIconButton>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaIconButton);

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("buttonSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ButtonSize { get; set; }

    [JsonPropertyName("buttonStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ButtonStyle { get; set; }

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? PrimaryAction { get; set; }

    [JsonPropertyName("vectorSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? VectorSource { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaIconButton
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            if (primaryActionProp is not null)
            {
                primaryActionProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}