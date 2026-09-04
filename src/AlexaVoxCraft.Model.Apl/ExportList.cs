using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class ExportList
{
    [JsonPropertyName("graphics")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Graphics { get; set; } = null!;

    [JsonPropertyName("layouts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Layouts { get; set; } = null!;

    [JsonPropertyName("resources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Resources { get; set; } = null!;

    [JsonPropertyName("styles")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Styles { get; set; } = null!;
}