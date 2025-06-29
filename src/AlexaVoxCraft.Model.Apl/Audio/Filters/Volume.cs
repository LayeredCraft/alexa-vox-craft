using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio.Filters;

public class Volume : APLAFilter
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Volume);

    [JsonPropertyName("amount")]
    public APLValue<double> Duration { get; set; }
}