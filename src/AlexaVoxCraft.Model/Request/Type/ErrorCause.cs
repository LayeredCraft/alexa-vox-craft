using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class ErrorCause
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; }
}