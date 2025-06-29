using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(APLDocumentConverter))]
public abstract class APLDocumentReference
{
    [JsonPropertyName("type")] public abstract string Type { get; }

    [JsonPropertyName("version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? VersionString { get; set; }
}