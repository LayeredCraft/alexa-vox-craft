using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.DataStore;

public class DataBinding
{
    [JsonPropertyName("namespace")] public string Namespace { get; set; } = null!;

    [JsonPropertyName("key")] public string Key { get; set; } = null!;

    [JsonPropertyName("dataBindingName")] public string DataBindingName { get; set; } = null!;

    [JsonPropertyName("dataType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DataBindingDataType? DataType { get; set; }

    [JsonPropertyName("startIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StartIndex { get; set; }

    [JsonPropertyName("endIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EndIndex { get; set; }
}