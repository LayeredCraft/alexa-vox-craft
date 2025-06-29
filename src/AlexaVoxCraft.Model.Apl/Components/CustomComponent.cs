using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class CustomComponent : APLComponent
{
    public CustomComponent(string type)
    {
        Type = type;
    }

    [JsonPropertyName("type")]
    public override string Type { get; }
}