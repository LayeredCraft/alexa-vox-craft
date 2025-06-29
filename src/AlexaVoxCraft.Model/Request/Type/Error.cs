using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Request.Type;

public class Error
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ErrorType>))]
    public ErrorType Type { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}