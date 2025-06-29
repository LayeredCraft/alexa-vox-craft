using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.DataStore;

public class DataStoreSettings
{
    [JsonPropertyName("dataBindings")] public List<DataBinding> DataBindings { get; set; } = [];
}