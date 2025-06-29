using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class ResolutionValue
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}