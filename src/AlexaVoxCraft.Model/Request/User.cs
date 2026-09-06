using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class User
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("permissions")]
    public Permissions Permissions { get; set; } = null!;
}