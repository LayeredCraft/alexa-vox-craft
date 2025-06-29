using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class DisplayElementSelectedRequest:Request
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}