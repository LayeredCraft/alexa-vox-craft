using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class InstallationError : Request.Type.Request
{
    public const string RequestType = "Alexa.DataStore.PackageManager.InstallationError";

    [JsonPropertyName("packageId")] public string PackageId { get; set; } = null!;

    [JsonPropertyName("version")] public string Version { get; set; } = null!;

    [JsonPropertyName("error")] public InstallationErrorDetail Error { get; set; } = null!;
}