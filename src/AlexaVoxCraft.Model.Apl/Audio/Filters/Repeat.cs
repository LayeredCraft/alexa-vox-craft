using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio.Filters;

public class Repeat : APLAFilter
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Repeat);

    [JsonPropertyName("repeatCount")]
    public APLValue<int> RepeatCount { get; set; }
}