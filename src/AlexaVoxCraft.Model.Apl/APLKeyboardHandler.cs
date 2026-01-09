using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLKeyboardHandler
{
    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? When { get; set; }

    [JsonPropertyName("propagate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Propagate { get; set; }

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Commands { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }
    public static void AddSupport()
    {
    }
}