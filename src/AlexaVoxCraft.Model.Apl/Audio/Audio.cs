using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Audio : APLAComponent
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Audio);

    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Source { get; set; }

    [JsonPropertyName("filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLAFilter>? Filters { get; set; }
}