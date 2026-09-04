using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class LocalePublishingInformationMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("keywords")]
    public string[] Keywords { get; set; } = null!;

    [JsonPropertyName("iconUri")]
    public string IconUri { get; set; } = null!;

    [JsonPropertyName("previews")]
    public string[] Previews { get; set; } = null!;
}