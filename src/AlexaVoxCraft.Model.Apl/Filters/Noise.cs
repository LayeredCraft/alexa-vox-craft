using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Filters;

public class Noise : IImageFilter
{
    [JsonPropertyName("type")] public string Type => nameof(Noise);

    [JsonPropertyName("useColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? UseColor { get; set; }

    [JsonPropertyName("sigma")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Sigma { get; set; }

    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<NoiseKind?>? Kind { get; set; }
}