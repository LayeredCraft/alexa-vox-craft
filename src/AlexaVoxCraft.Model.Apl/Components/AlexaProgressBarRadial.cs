using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaProgressBarRadial : AlexaProgressBarBase, IJsonSerializable<AlexaProgressBarRadial>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaProgressBarRadial);
    public new static void RegisterTypeInfo<T>() where T : AlexaProgressBarRadial
    {
        AlexaProgressBarBase.RegisterTypeInfo<T>();
    }
}