using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class APLPackage
{
    [JsonPropertyName("packageVersion")]
    public PackageVersion PackageVersion { get; set; }

    [JsonPropertyName("packageType")] public string PackageType { get; } = "APL_PACKAGE";

    [JsonPropertyName("publishingInformation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PublishingInformation? PublishingInformation { get; set; }

    [JsonPropertyName("manifest")]
    public Manifest Manifest { get; set; }
}