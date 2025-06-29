using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio.Filters;

public class Trim : APLAFilter
{
    [JsonPropertyName("type")] public override string Type => nameof(Trim);

    [JsonPropertyName("start")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Start { get; set; }

    [JsonPropertyName("end")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? End { get; set; }
}