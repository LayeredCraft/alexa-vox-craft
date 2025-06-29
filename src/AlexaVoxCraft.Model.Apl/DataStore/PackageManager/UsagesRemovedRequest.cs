using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class UsagesRemovedRequest : Request.Type.Request
{
    public const string RequestType = "Alexa.DataStore.PackageManager.UsagesRemoved";

    [JsonPropertyName("payload")] public UsagesInstalledPayload Payload { get; set; }
}