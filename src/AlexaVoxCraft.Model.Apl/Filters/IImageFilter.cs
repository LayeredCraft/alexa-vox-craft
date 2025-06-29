using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.Filters;

[JsonConverter(typeof(ImageFilterConverter))]
public interface IImageFilter
{
    [JsonPropertyName("type")]
    string Type { get; }
}