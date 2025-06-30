using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaProgressDots : APLComponent, IJsonSerializable<AlexaProgressDots>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaProgressDots);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("dotSize")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? DotSize { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaProgressDots
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}