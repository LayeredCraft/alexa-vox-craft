using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Selector : APLAMultiChildComponent, IJsonSerializable<Selector>
{
    [JsonPropertyName("strategy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SelectorStrategy?>? Strategy { get; set; }

    [JsonPropertyName("type")]
    public override string Type => nameof(Selector);

    public static void RegisterTypeInfo<T>() where T : Selector
    {
        APLAMultiChildComponent.RegisterTypeInfo<T>();
    }
}