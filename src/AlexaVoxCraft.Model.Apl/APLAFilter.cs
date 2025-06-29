using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(APLAFilterConverter))]
public abstract class APLAFilter
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }

    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public APLValue<bool?>? When { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }
}