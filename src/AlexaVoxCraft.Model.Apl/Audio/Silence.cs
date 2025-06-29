using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Silence : APLAComponent
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Silence);

    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public new APLValue<int?>? Duration { get; set; }
}