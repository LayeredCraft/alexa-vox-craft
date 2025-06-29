using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreDeviceErrorContent : DataStoreErrorContent
{
    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<DataStoreCommand> Commands { get; set; }
}