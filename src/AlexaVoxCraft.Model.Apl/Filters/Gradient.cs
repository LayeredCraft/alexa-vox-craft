using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Filters;

public class Gradient : IImageFilter
{
    [JsonPropertyName("type")] public string Type => nameof(Gradient);

    [JsonPropertyName("gradient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<APLGradient>? SelectedGradient { get; set; }
}