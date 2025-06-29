using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Audio;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<SpeechContentType>))]
public enum SpeechContentType
{
    [EnumMember(Value="PlainText")]
    PlainText,
    [EnumMember(Value="SSML")]
    Ssml
}