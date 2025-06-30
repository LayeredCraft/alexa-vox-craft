using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class ConnectionSendRequest<T> : ConnectionSendRequest
{
    [JsonPropertyName("payload")]
    public T Payload { get; set; }
}

public class ConnectionSendRequest:IDirective
{
    public const string DirectiveType = "Connections.SendRequest";
    [JsonPropertyName("type")] public string Type => DirectiveType;

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("token")]
    string Token { get; set; }
}