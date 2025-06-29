using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class PaginationContext
{
    [JsonPropertyName("totalCount")] public int TotalCount { get; set; }

    [JsonPropertyName("nextToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string NextToken { get; set; }

    [JsonPropertyName("previousToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PreviousToken { get; set; }
}