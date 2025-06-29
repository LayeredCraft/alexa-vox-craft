using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Person
{
    [JsonPropertyName("personId")]
    public string PersonId { get; set; }

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("authenticationConfidenceLevel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AuthenticationConfidenceLevel? AuthenticationConfidenceLevel { get; set; }
}