using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.ConnectionTasks;

public class ConnectionTaskContext
{
    [JsonPropertyName("providerId")]
    public string ProviderId { get; set; }
}