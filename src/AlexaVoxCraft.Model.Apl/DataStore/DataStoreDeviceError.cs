using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreDeviceError : DataStoreError
{
    [JsonPropertyName("content")] public DataStoreDeviceErrorContent Content { get; set; }
}