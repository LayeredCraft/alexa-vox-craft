using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Scope
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
}