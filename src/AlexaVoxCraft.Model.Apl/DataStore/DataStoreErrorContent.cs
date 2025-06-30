using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreErrorContent
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; }
}