using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Request.Type;

public class SessionEndedRequest : Request
{
    [JsonPropertyName("reason")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<Reason>))]
    public Reason Reason { get; set; }

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Error? Error { get; set; }
}