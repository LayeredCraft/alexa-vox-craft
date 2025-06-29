using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ScrollableTagDirection>))]
public enum ScrollableTagDirection
{
    [EnumMember(Value="horizontal")]
    Horizontal,
    [EnumMember(Value="vertical")]
    Vertical
}