using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class ResolutionStatus
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
}