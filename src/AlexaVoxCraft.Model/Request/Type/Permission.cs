using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class Permission
{
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}