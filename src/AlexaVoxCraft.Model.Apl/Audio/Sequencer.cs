using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Sequencer : APLAMultiChildComponent, IJsonSerializable<Sequencer>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Sequencer);

    public static void RegisterTypeInfo<T>() where T : Sequencer
    {
        APLAMultiChildComponent.RegisterTypeInfo<T>();
    }
}