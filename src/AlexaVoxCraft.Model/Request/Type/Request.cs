using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Helpers;

namespace AlexaVoxCraft.Model.Request.Type;

[JsonConverter(typeof(RequestConverter))]
public abstract class Request
{
    [JsonPropertyName("type")]
    [JsonRequired]
    public string Type { get; set; } = null!;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = null!;

    [JsonPropertyName("locale")]
    public string Locale { get; set; } = null!;

    [JsonPropertyName("timestamp"), JsonConverter(typeof(MixedDateTimeConverter))]
    public DateTime Timestamp { get; set; }
}