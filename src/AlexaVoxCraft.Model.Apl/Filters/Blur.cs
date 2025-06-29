using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Filters;

public class Blur : IImageFilter
{
    [JsonPropertyName("radius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue Radius { get; set; }

    public Blur()
    {
    }

    public Blur(Dimension radius)
    {
        Radius = radius;
    }

    [JsonPropertyName("type")]
    public string Type => nameof(Blur);
}