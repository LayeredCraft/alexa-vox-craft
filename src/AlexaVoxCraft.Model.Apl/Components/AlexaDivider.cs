using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaDivider : APLComponent, IJsonSerializable<AlexaDivider>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaDivider);

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Theme { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaDivider
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}