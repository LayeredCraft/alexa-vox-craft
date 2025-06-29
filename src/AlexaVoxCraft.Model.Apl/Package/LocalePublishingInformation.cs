using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class LocalePublishingInformation
{
    [JsonPropertyName("targetViewport")]
    public TargetViewport TargetViewport { get; set; }

    [JsonPropertyName("metadata")]
    public LocalePublishingInformationMetadata Metadata { get; set; }
}