using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Person
{
    [JsonPropertyName("personId")]
    public string PersonId { get; set; } = null!;

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("authenticationConfidenceLevel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AuthenticationConfidenceLevel? AuthenticationConfidenceLevel { get; set; }
}