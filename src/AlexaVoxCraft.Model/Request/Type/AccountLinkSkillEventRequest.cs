using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class AccountLinkSkillEventRequest : SkillEventRequest
{
    [JsonPropertyName("body")] public AccountLinkSkillEventDetail Body { get; set; }
}