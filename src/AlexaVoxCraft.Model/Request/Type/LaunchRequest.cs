using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class LaunchRequest : Request
{
    [JsonPropertyName("task")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LaunchRequestTask Task { get; set; }
}