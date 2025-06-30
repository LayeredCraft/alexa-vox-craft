using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringExportConverter))]
public class Export
{
    public Export()
    {
    }

    public Export(string name)
    {
        Name = name;
    }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Description { get; set; }

    public static implicit operator Export(string exportName)
    {
        return new Export(exportName);
    }
}