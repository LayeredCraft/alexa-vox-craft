using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class PublishingInformation
{
    [JsonPropertyName("schemaVersion")] public string SchemaVersion { get; set; } = "1.0";

    [JsonPropertyName("locales")]
    public Dictionary<string, List<LocalePublishingInformation>> Locales { get; set; } = new();
}