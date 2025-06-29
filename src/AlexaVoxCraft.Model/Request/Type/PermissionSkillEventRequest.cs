using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class PermissionSkillEventRequest : SkillEventRequest
{
    [JsonPropertyName("body")] public SkillEventPermissions Body { get; set; }
}