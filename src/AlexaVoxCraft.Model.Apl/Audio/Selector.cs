using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Selector : APLAMultiChildComponent
{
    [JsonPropertyName("strategy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SelectorStrategy?>? Strategy { get; set; }

    [JsonPropertyName("type")]
    public override string Type => nameof(Selector);
}