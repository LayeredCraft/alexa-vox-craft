using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLCommandSource
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Type { get; set; } = null!;

    [JsonPropertyName("handler")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Handler { get; set; } = null!;

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ComponentId { get; set; } = null!;
}