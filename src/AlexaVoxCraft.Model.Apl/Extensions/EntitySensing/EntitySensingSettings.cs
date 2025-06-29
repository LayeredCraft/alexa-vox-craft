using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.EntitySensing;

public class EntitySensingSettings
{
    [JsonPropertyName("entitySensingStateName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string EntitySensingStateName { get; set; }

    [JsonPropertyName("primaryUserName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PrimaryUserName { get; set; }
}