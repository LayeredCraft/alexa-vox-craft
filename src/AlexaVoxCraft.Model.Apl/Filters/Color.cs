using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Filters;

public class Color : IImageFilter
{
    [JsonPropertyName("type")] public string Type => nameof(Color);

    [JsonPropertyName("color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SelectedColor { get; set; }
}