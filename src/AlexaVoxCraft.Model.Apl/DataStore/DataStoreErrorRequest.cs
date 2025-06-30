using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreErrorRequest : Request.Type.Request
{
    public const string RequestType = "Alexa.DataStore.DataStoreError";

    [JsonPropertyName("error")] public DataStoreError Error { get; set; }
}