using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(ViewportConverter))]
public abstract class Viewport
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }

    [JsonPropertyName("id")]
    public string ID { get; set; }
}