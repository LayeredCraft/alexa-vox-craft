using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Filters;

public class Blend : IImageFilter
{
    [JsonPropertyName("type")] public string Type => nameof(Blend);

    [JsonPropertyName("destination")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Destination { get; set; }

    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Source { get; set; }

    [JsonPropertyName("mode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<BlendMode?>? Mode { get; set; }
}