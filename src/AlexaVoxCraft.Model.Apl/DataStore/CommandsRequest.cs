using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class CommandsRequest
{
    [JsonPropertyName("commands")] public List<DataStoreCommand> Commands { get; set; } = [];

    [JsonPropertyName("target")] public CommandsTarget Target { get; set; } = new();

    [JsonPropertyName("attemptDeliveryUntil")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AttemptDeliveryUntil { get; set; }
}