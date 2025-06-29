using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<APLTFormat>))]
public enum APLTFormat
{
    [EnumMember(Value="SEVEN_SEGMENT")]
    SevenSegment
}