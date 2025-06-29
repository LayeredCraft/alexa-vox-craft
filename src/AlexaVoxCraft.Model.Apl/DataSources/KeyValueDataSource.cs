using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataSources;

public class KeyValueDataSource:APLDataSource
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public override string Type { get; }

    [JsonExtensionData]
    public Dictionary<string,JsonElement> Properties { get; set; }
}