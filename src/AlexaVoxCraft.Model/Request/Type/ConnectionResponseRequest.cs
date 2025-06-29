using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class ConnectionResponseRequest<T> : ConnectionResponseRequest
{
    [JsonPropertyName("payload")]
    public T Payload { get; set; }
}

public class ConnectionResponseRequest:Request
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("status")]
    public ConnectionStatus Status { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }
}