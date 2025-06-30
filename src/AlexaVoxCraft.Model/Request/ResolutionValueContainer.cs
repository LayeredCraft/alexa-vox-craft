using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class ResolutionValueContainer
{
    [JsonPropertyName("value")]
    public ResolutionValue Value { get; set; }
}