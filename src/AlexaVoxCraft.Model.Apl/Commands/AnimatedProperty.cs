using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.Commands;

[JsonConverter(typeof(AnimatedPropertyConverter))]
public abstract class AnimatedProperty
{
    [JsonPropertyName("property")]
    public abstract APLValue<string> Property { get; }
}