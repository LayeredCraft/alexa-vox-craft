using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class SkillEventPermissions
{
    [JsonPropertyName("acceptedPermissions")]
    public Permission[] AcceptedPermissions { get; set; }
}