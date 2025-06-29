using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class LocalePublishingInformationMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("keywords")]
    public string[] Keywords { get; set; }

    [JsonPropertyName("iconUri")]
    public string IconUri { get; set; }

    [JsonPropertyName("previews")]
    public string[] Previews { get; set; }
}