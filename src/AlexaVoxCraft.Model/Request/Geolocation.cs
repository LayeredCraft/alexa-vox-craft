using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Request;

public class Geolocation
{
    [JsonPropertyName("locationServices")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LocationServices? LocationServices { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }
    [JsonPropertyName("coordinate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeolocationCoordinate? Coordinate { get; set; }
    [JsonPropertyName("altitude")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeolocationAltitude? Altitude { get; set; }
    [JsonPropertyName("heading")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeolocationHeading? Heading { get; set; }
    [JsonPropertyName("speed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeolocationSpeed? Speed { get; set; }
}