using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.ConnectionTasks;

namespace AlexaVoxCraft.Model.Request.Type;

public class LaunchRequestTask
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("version")]
    public string Version { get; set; } = null!;

    [JsonPropertyName("input")]
    public IConnectionTask Input { get; set; } = null!;
}