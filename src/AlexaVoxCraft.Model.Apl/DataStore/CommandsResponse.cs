using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class CommandsResponse
{
    [JsonPropertyName("results")]
    public CommandResult[] Results { get; set; } = null!;

    [JsonPropertyName("queuedResultId")]
    public string QueuedResultId { get; set; } = null!;
}