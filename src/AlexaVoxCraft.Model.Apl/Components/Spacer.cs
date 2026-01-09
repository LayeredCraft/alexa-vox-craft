using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Spacer : APLComponent, IJsonSerializable<Spacer>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Spacer);

    public new static void RegisterTypeInfo<T>() where T : Spacer
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}