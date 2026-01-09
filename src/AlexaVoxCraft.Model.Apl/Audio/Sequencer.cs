using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Sequencer : APLAMultiChildComponent
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Sequencer);
}