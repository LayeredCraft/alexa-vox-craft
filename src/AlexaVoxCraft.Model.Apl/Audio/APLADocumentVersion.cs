using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Audio;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<APLADocumentVersion>))]
public enum APLADocumentVersion
{
    Unknown,
    [EnumMember(Value="0.8")]
    V0_8,
    [EnumMember(Value = "0.9")]
    V0_9
}