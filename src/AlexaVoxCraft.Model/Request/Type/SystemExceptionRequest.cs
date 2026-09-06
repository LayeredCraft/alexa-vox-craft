using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class SystemExceptionRequest : Request
{
    [JsonPropertyName("error")]
    public Error Error { get; set; } = null!;
    [JsonPropertyName("cause")]
    public ErrorCause ErrorCause { get; set; } = null!;
}