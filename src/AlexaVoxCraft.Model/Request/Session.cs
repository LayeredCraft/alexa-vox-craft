using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Session
{
    [JsonPropertyName("new")]
    public bool New { get; set; }

    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = null!;

    [JsonPropertyName("attributes")]
    public Dictionary<string, JsonElement>? Attributes { get; set; }

    [JsonPropertyName("application")]
    public Application Application { get; set; } = null!;

    [JsonPropertyName("user")]
    public User User { get; set; } = null!;
}