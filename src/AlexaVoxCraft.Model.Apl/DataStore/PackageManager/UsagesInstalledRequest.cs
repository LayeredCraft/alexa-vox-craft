using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class UsagesInstalledRequest : Request.Type.Request
{
    public const string RequestType = "Alexa.DataStore.PackageManager.UsagesInstalled";

    [JsonPropertyName("payload")] public UsagesInstalledPayload Payload { get; set; }
}