using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class PlaybackState
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    [JsonPropertyName("offsetInMilliseconds")]
    public long OffsetInMilliseconds { get; set; }

    [JsonPropertyName("playerActivity")]
    public string PlayerActivity { get; set; } = null!;
}