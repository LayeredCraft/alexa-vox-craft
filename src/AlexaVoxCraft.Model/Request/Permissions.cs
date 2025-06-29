using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Permissions
{
    [JsonPropertyName("consentToken"),Obsolete("ConsentToken is deprecated, please use SkillRequest.Context.System.ApiAccessToken")]
    public string ConsentToken { get; set; }

    [JsonPropertyName("scopes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Scope>? Scopes { get; set; }
}