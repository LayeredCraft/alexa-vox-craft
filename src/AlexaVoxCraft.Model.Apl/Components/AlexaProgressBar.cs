using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaProgressBar : AlexaProgressBarBase, IJsonSerializable<AlexaProgressBar>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaProgressBar);
    public new static void RegisterTypeInfo<T>() where T : AlexaProgressBar
    {
        AlexaProgressBarBase.RegisterTypeInfo<T>();
    }
}