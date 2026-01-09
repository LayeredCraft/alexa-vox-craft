using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Select : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => "Select";

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Commands { get; set; }

    [JsonPropertyName("otherwise")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Otherwise { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data { get; set; }
}