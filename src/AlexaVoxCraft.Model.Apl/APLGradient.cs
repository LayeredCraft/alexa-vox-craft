using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLGradient
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public APLGradientType? Type { get; set; }

    [JsonPropertyName("angle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public APLValue<int?>? Angle { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("colorRange")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public APLValueCollection<string>? ColorRange { get; set; }

    [JsonPropertyName("inputRange")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public APLValueCollection<double>? InputRange { get; set; }
}