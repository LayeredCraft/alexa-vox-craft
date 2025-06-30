using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public class AVGParameter
{
    [JsonPropertyName("name")] public APLValue<string> Name { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AVGParameterType>? Type { get; set; }

    [JsonPropertyName("default")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Default { get; set; }
}