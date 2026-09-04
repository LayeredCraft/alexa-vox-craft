using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class InstalledPackage
{
    [JsonPropertyName("packageId")]
    public string PackageId { get; set; } = null!;

    [JsonPropertyName("packageVersion")]
    public string PackageVersion { get; set; } = null!;
}