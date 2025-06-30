using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class SessionResumedRequest:Request
{
    [JsonPropertyName("originIpAddress")]
    public string OriginIpAddress { get; set; }

    [JsonPropertyName("cause")]
    public SessionResumedRequestCause Cause { get; set; }
}