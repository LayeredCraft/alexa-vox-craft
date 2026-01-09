using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Mixer : APLAMultiChildComponent, IJsonSerializable<Mixer>
{
    [JsonPropertyName("type")] public override string Type => nameof(Mixer);
    public static void RegisterTypeInfo<T>() where T : Mixer
    {
        APLAMultiChildComponent.RegisterTypeInfo<T>();
    }
}