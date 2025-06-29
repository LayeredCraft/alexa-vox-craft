using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class AlexaDisplay
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}