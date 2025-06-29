using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio.Filters;

public class FadeIn : APLAFilter
{
    [JsonPropertyName("type")]
    public override string Type => nameof(FadeIn);

    [JsonPropertyName("duration")]
    public APLValue<int> Duration { get; set; }
}