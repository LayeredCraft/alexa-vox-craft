using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class PackageManagerContext
{
    [JsonPropertyName("installedPackages")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InstalledPackage[] InstalledPackages { get; set; }
}