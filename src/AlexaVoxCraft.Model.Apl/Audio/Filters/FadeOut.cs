using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio.Filters;

public class FadeOut : APLAFilter
{
    [JsonPropertyName("type")]
    public override string Type => nameof(FadeOut);

    [JsonPropertyName("duration")]
    public APLValue<int> Duration { get; set; }
}