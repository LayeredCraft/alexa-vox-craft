using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class UpdateRequest : Request.Type.Request
{
    public const string RequestType = "Alexa.DataStore.PackageManager.UpdateRequest";

    [JsonPropertyName("fromVersion")] public string FromVersion { get; set; }

    [JsonPropertyName("toVersion")] public string ToVersion { get; set; }

    [JsonPropertyName("packageId")] public string PackageId { get; set; }
}