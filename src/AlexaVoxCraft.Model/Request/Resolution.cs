using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Resolution
{
    [JsonPropertyName("resolutionsPerAuthority")]
    public ResolutionAuthority[] Authorities { get; set; }
}