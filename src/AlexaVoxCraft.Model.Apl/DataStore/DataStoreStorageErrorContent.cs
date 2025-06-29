using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreStorageErrorContent : DataStoreErrorContent
{
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    [JsonPropertyName("failedCommand")] public DataStoreCommand FailedCommand { get; set; }
}