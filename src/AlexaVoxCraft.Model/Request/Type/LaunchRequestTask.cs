using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.ConnectionTasks;

namespace AlexaVoxCraft.Model.Request.Type;

public class LaunchRequestTask
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("input")]
    public IConnectionTask Input { get; set; }
}