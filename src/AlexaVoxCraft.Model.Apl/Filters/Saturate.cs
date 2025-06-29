using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Filters;

public class Saturate : IImageFilter
{
    [JsonPropertyName("type")] public string Type => nameof(Saturate);

    [JsonPropertyName("amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? Amount { get; set; }

    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Source { get; set; }
}