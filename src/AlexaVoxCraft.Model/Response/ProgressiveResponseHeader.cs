using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response;

public class ProgressiveResponseHeader
{
    public ProgressiveResponseHeader(string requestId)
    {
        RequestId = requestId;
    }

    [JsonPropertyName("requestId")]
    public string RequestId { get; }
}