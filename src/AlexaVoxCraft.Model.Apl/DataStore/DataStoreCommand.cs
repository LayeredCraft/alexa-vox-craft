using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.DataStore;

[JsonConverter(typeof(DataStoreCommandConverter))]
public abstract class DataStoreCommand
{
    public DataStoreCommand(string type) => Type = type;

    [JsonPropertyName("type")]
    public string Type { get; }
}