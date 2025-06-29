using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Application
{
    [JsonPropertyName("applicationId")]
    public string ApplicationId { get; set; }
}