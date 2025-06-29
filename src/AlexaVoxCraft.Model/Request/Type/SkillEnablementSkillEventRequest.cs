using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class SkillEnablementSkillEventRequest: SkillEventRequest
{
    [JsonPropertyName("body")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SkillEventPersistenceStatus? Body { get; set; }
}