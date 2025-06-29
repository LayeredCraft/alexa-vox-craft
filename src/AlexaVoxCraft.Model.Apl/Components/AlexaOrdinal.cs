using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaOrdinal : APLComponent, IJsonSerializable<AlexaOrdinal>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaOrdinal);

    [JsonPropertyName("ordinalText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> OrdinalText { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Theme { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaOrdinal
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}