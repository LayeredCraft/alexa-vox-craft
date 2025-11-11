using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.ConnectionTasks;

namespace AlexaVoxCraft.Model.Tests.Examples;

public class ExampleTask : IConnectionTask
{
    public string ConnectionUri { get; set; }
    [JsonPropertyName("randomParameter")]
    public string RandomParameter { get; set; }
}