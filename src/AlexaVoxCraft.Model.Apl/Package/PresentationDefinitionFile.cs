using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class PresentationDefinitionFile
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}