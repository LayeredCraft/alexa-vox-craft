using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class QueuedResultResponse
{
    [JsonPropertyName("items")] public CommandResult[] Items { get; set; }

    [JsonPropertyName("paginationContext")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationContext PaginationContext { get; set; }
}