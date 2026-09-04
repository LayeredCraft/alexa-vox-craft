using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class VisibilityChangeHandler
{
    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> When { get; set; } = null!;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Description { get; set; } = null!;

    [JsonPropertyName("commands")] public APLValueCollection<APLCommand> Commands { get; set; } = null!;
}