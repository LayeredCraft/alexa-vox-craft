using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.DataStore;

[JsonConverter(typeof(DataStoreErrorConverter))]
public abstract class DataStoreError
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
}