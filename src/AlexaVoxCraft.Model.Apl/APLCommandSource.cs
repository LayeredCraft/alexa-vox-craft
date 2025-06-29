using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLCommandSource
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Type { get; set; }

    [JsonPropertyName("handler")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Handler { get; set; }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ComponentId { get; set; }
}