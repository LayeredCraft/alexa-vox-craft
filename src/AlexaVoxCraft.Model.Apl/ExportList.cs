using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class ExportList
{
    [JsonPropertyName("graphics")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Graphics { get; set; }

    [JsonPropertyName("layouts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Layouts { get; set; }

    [JsonPropertyName("resources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Resources { get; set; }

    [JsonPropertyName("styles")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Export[] Styles { get; set; }
}