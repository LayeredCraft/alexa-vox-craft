using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;

[JsonConverter(typeof(IAVGFilterConverter))]
public interface IAVGFilter
{
    [JsonPropertyName("type")]
    string Type { get; }
}