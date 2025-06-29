using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Session
{
    [JsonPropertyName("new")]
    public bool New { get; set; }

    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, object> Attributes { get; set; }

    [JsonPropertyName("application")]
    public Application Application { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }
}