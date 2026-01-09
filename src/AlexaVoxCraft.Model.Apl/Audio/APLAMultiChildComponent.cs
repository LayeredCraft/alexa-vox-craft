using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public abstract class APLAMultiChildComponent : APLAComponent
{
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLAComponent>? Items { get; set; }
}