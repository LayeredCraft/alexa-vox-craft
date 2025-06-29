using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model;

public class ConnectionStatus
{
    public ConnectionStatus() { }

    public ConnectionStatus(int code, string message)
    {
        Code = code;
        Message = message;
    }

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}