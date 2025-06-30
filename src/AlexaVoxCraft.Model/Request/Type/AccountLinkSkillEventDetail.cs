using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class AccountLinkSkillEventDetail
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
}