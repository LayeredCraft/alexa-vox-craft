using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Request;

public class Context
{
    [JsonPropertyName("System")]
    public AlexaSystem System { get; set; }
        
    [JsonPropertyName("AudioPlayer")]
    public PlaybackState AudioPlayer { get; set; }

    [JsonPropertyName("Geolocation")]
    public Geolocation Geolocation { get; set; }
}