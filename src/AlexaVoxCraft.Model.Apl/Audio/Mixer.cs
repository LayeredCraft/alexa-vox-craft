using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Mixer : APLAMultiChildComponent
{
    [JsonPropertyName("type")] public override string Type => nameof(Mixer);
}