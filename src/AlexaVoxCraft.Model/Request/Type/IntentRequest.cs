using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class IntentRequest : Request
{
    [JsonPropertyName("dialogState")]
    public string DialogState { get; set; }

    [JsonPropertyName("intent")]
    public Intent Intent { get; set; }
}