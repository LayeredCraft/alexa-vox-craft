using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class TickHandler
{
    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? When { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("minimumDelay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int>? MinimumDelay { get; set; }

    [JsonPropertyName("commands")] public APLValueCollection<APLCommand> Commands { get; set; }
}