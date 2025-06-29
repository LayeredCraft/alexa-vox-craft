using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreStorageError : DataStoreError
{
    [JsonPropertyName("content")] public DataStoreStorageErrorContent Content { get; set; }
}