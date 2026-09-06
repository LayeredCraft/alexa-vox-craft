using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class ConnectionResponseRequest<T> : ConnectionResponseRequest
{
    [JsonPropertyName("payload")]
    public T Payload { get; set; } = default!;
}

public class ConnectionResponseRequest : Request
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("status")]
    public ConnectionStatus Status { get; set; } = null!;

    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;
}