using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class SessionResumedRequestCause
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    [JsonPropertyName("status")]
    public ConnectionStatus Status { get; set; } = null!;

    [JsonPropertyName("result")]
    public object Result { get; set; } = null!;
}