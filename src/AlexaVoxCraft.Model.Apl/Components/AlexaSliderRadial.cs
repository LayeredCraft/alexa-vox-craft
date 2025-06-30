using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaSliderRadial : AlexaSliderBase, IJsonSerializable<AlexaSliderRadial>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaSliderRadial);

    [JsonPropertyName("useDefaultSliderExpandTransition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? UseDefaultSliderExpandTransition { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaSliderRadial
    {
        AlexaSliderBase.RegisterTypeInfo<T>();
    }
}