using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class LoadIndexListDataRequest : Request.Type.Request
{
    public const string RequestType = "Alexa.Presentation.APL.LoadIndexListData";

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }

    [JsonPropertyName("correlationToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CorrelationToken { get; set; }

    [JsonPropertyName("listId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ListId { get; set; }

    [JsonPropertyName("startIndex")] public int StartIndex { get; set; }

    [JsonPropertyName("count")] public int Count { get; set; }
}